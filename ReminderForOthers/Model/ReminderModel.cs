
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using Plugin.CloudFirestore;
using Plugin.FirebaseStorage;

namespace ReminderForOthers.Model
{
    public class Reminder
    {
        public int Id { get; set; }
        public string UsernameFrom { get; set; }
        public string UsernameTo { get; set; }
        public string Title { get; set; }
        public DateTime PlayDateTime { get; set; }
        public string RecordPath { get; set; }
        public DateTime ReminderCreationTime { get; set; }
        public bool HasPlayed { get; set; }
    }
    public class ReminderModel
    {
        private const string Web_API_KEY = "AIzaSyDzaUbGPZSIoOknBEOLNOA5KbVGstQ1Sks";
        private const string Database_URL = "https://reminderforothers-default-rtdb.asia-southeast1.firebasedatabase.app";
        private FirebaseClient client = new FirebaseClient(Database_URL);

        //recordings
        private Reminder reminder;
        private readonly string fileName = "Reminders.txt";

        public string filePath { get; }

        public ReminderModel()
        {
            filePath = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);
        }

        public ReminderModel(Reminder reminder)
        {
            this.reminder = reminder;

            Console.WriteLine("PlayDateTime: " + reminder.PlayDateTime);
            filePath = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);
        }

        //store reminder
        public async Task<bool> StoreReminderAsync()
        {
            //add into data
            //bool storedFlag = await AddReminderLocallyAsync();
            return await AddReminderCloudAsync();
        }


        //add reminder firebase realtime db
        private async Task<bool> AddReminderCloudAsync()
        {
            //get reminders first
            List<Reminder> reminders = ConvertToListReminder(await GetReceivedRemindersAsync(reminder.UsernameTo));
            reminder.Id = reminders.Count + 1;

            //upload audio file to firebase storage cloud
            await UploadAudioToCloud();

            //add reminder to cloud
            //var response = await client.Child(nameof(Reminder)).PostAsync(reminder);
            bool response = await SetReminderFirestore();

            if (!response) { return false; }

            //Console.WriteLine("Reminder key: " + response.Key);
            return true;
        }

        //get reminder for userTo
        public async Task<IDictionary<string, Reminder>> GetReceivedRemindersAsync(string userTo)
        {
            var queryUserTo = await CrossCloudFirestore.Current
                                                 .Instance
                                                 .Collection("reminder")
                                                 .WhereEqualsTo("UsernameTo", userTo)
                                                 .GetAsync();
            var remindersSent = queryUserTo.ToObjects<Reminder>();

            //returns dictionary after sorting (key : documentID) : (value: Reminder)
            string[] documentIDs = GetDocumentIDs(queryUserTo.Documents.ToList());
            return SortRemindersByDateAsc(documentIDs, remindersSent.ToArray<Reminder>());
        }

        //get reminder that user sent
        public async Task<IDictionary<string,Reminder>> GetSentRemindersAsync(string userFrom)
        {
            var queryUserFrom = await CrossCloudFirestore.Current
                                                 .Instance
                                                 .Collection("reminder")
                                                 .WhereEqualsTo("UsernameFrom", userFrom)
                                                 .GetAsync();
            var remindersSent = queryUserFrom.ToObjects<Reminder>();

            //returns dictionary after sorting (key : documentID) : (value: Reminder)
            string[] documentIDs = GetDocumentIDs(queryUserFrom.Documents.ToList());
            return SortRemindersByDateAsc(documentIDs, remindersSent.ToArray<Reminder>());

        }

        //helper method for returning documents ids
        private string[] GetDocumentIDs(List<IDocumentSnapshot> documents)
        {
            List<string> documentIDs = new List<string>();
            documents.ForEach((item) => { documentIDs.Add(item.Id); });
            return documentIDs.ToArray();
        }

        //helper method to re arrange according to date and time
        private IDictionary<string, Reminder> SortRemindersByDateAsc(string[] documentIDs, Reminder[] reminders)
        {
            for (int i = 0; i < reminders.Length; i++)
            {
                for (int j = reminders.Length - 1; j > i; j--)
                {
                    Console.WriteLine("Reminder: " + reminders[i].PlayDateTime);
                    //Console.WriteLine($"Before switch, Reminder i:{reminders[i].Id}, Reminder j: {reminders[j].Id}");
                    if (reminders[i].PlayDateTime >= reminders[j].PlayDateTime)
                    {
                        //rearrange reminder
                        Reminder reTemp = reminders[i];
                        reminders[i] = reminders[j];
                        reminders[j] = reTemp;

                        //rearrange documentId
                        string idTemp = documentIDs[i];
                        documentIDs[i] = documentIDs[j];
                        documentIDs[j] = idTemp;

                        //Console.WriteLine($"After switch, Reminder i:{reminders[i].Id}, Reminder j: {reminders[j].Id}");
                    }
                }
            }

            return ConvertToDictionary(documentIDs,reminders);
        }


        //conver to IDictionary
        private IDictionary<string, Reminder> ConvertToDictionary(string[] documentIDs, Reminder[] reminders) 
        {
            IDictionary<string, Reminder> remindersDic = new Dictionary<string, Reminder>();

            for (int i = 0; i < reminders.Length; i++)
            {
                DateTime newDate = reminders[i].PlayDateTime;
                reminders[i].PlayDateTime = newDate.ToLocalTime();
                remindersDic.Add(documentIDs[i], reminders[i]);
            }

            return remindersDic;
        }

        //convert from IDictionary To Reminder List
        public List<Reminder> ConvertToListReminder(IDictionary<string, Reminder> remindersDic)
        {
            List<Reminder> reminders = new List<Reminder>();

            foreach (var item in remindersDic)
            {
                Reminder temp = item.Value;
                reminders.Add(temp);
            }

            return reminders;
        }

        //convert from IDictionary To documentID List
        public List<string> ConvertToListDocumentID(IDictionary<string, Reminder> remindersDic)
        {
            List<string> documentIDs = new List<string>();

            foreach (var item in remindersDic)
            {
                string temp = item.Key;
                documentIDs.Add(temp);
            }

            return documentIDs;
        }

        //get audio from cloud
        public async Task<string> GetAudioFilePathAsync(string path)
        {
            string fileName = path.Split("/")[1];
            string filePath = Path.Combine(FileSystem.Current.CacheDirectory, fileName);

            try
            {
                var reference = CrossFirebaseStorage.Current.Instance.RootReference.Child(path);
                await reference.GetFileAsync(filePath);
                //Console.WriteLine("Reference: " + reference.Name);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return null;
            }

            return filePath;
        }


        //add file to firebase storage
        public async Task<bool> UploadAudioToCloud()
        {
            string recordedPath = reminder.RecordPath;
            string[] recordPathArr = reminder.RecordPath.Split("/");
            string newRecordPath = "/reminders/" + recordPathArr[recordPathArr.Length - 1];
            Console.WriteLine("RecordPath: " + newRecordPath);

            try
            {
                var reference = CrossFirebaseStorage.Current.Instance.RootReference.Child(newRecordPath);
                await reference.PutFileAsync(reminder.RecordPath);
                reminder.RecordPath = newRecordPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;

        }


        //add file to firestore
        public async Task<bool> SetReminderFirestore()
        {
            try
            {
                var doc = await CrossCloudFirestore.Current
                          .Instance
                          .Collection("reminder")
                          .AddAsync(reminder);
                //Console.WriteLine(doc.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        //update reminder
        public async Task<bool> UpdateReminderFirestore(string documentID, Reminder reminder) 
        {
            try
            {
                await CrossCloudFirestore.Current
                          .Instance
                          .Collection("reminder")
                          .Document(documentID)
                          .UpdateAsync(reminder);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        //remove from database

        public async Task<bool> RemoveReminderFirestore(string documentID, Reminder reminder)
        {
            try
            {
                var reference = CrossFirebaseStorage.Current.Instance.RootReference.Child(reminder.RecordPath);
                await reference.DeleteAsync();

                await CrossCloudFirestore.Current
                          .Instance
                          .Collection("reminder")
                          .Document(documentID)
                          .DeleteAsync();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

    }
}
