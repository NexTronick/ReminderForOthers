
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

        public ReminderModel(string usernameFrom, string usernameTo, string title, DateTime date, TimeSpan time, string recordPath)
        {
            //set all the values for reminders
            reminder = new Reminder();
            reminder.UsernameFrom = usernameFrom;
            reminder.UsernameTo = usernameTo;
            reminder.Title = title;
            reminder.PlayDateTime = date.AddTicks(time.Ticks);
            reminder.RecordPath = recordPath;
            reminder.ReminderCreationTime = DateTime.Now;

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
            List<Reminder> reminders = await GetReceivedRemindersAsync(reminder.UsernameTo);
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
        public async Task<List<Reminder>> GetReceivedRemindersAsync(string userTo)
        {
            var queryUserTo = await CrossCloudFirestore.Current
                                                 .Instance
                                                 .Collection("reminder")
                                                 .WhereEqualsTo("UsernameTo", userTo)
                                                 .GetAsync();

            var remindersReceived = queryUserTo.ToObjects<Reminder>();
            foreach (var item in remindersReceived)
            {
                Console.WriteLine($"GetReceivedRemindersAsync UsernameFrom: {item.UsernameFrom} to UsernameTo: {item.UsernameTo}");
            }
            return remindersReceived.ToList<Reminder>();
        }
        //get reminder that user sent
        public async Task<List<Reminder>> GetSentRemindersAsync(string userFrom)
        {
            var queryUserFrom = await CrossCloudFirestore.Current
                                                 .Instance
                                                 .Collection("reminder")
                                                 .WhereEqualsTo("UsernameFrom", userFrom)
                                                 .GetAsync();

            var remindersSent = queryUserFrom.ToObjects<Reminder>();
            foreach (var item in remindersSent)
            {
                Console.WriteLine($"GetSentRemindersAsync UsernameFrom: {item.UsernameFrom} to UsernameTo: {item.UsernameTo}");
            }
            Console.WriteLine($"GetSentRemindersAsync remindersSent: {remindersSent.Count()}");
            return remindersSent.ToList<Reminder>();

        }

        //get audio from cloud
        public async Task<string> GetAudioFilePathAsync(string path) 
        {
            string fileName = path.Split("/")[1];
            string filePath = Path.Combine(FileSystem.Current.CacheDirectory, fileName);

            var reference = CrossFirebaseStorage.Current.Instance.RootReference.Child(path);
            //var downloadProgress = new Progress<IDownloadState>();
            //downloadProgress.ProgressChanged += (sender, e) =>
            //{
            //    var progress = e.TotalByteCount > 0 ? 100.0 * e.BytesTransferred / e.TotalByteCount : 0;
            //};

            //var stream = await reference.GetStreamAsync(downloadProgress);

            await reference.GetFileAsync(filePath);
            Console.WriteLine("Reference: "+ reference.Name);
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
                Console.WriteLine(doc.Id);
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
