
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;

using Newtonsoft.Json;

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
            IDictionary<string, Reminder> reminders = await GetRemindersForUserAsync(reminder.UsernameTo);
            reminder.Id = reminders.Count+1;

            //add now
            var response = await client.Child(nameof(Reminder)).PostAsync(reminder);
            if (response.Key == null) { return false; }

            Console.WriteLine("Reminder key: "+ response.Key);
            return true;
        }

        //get reminder for userTo
        public async Task<IDictionary<string, Reminder>> GetRemindersForUserAsync(string userTo)
        {
            var remindersDB = await client.Child(nameof(Reminder)).OnceAsync<Reminder>();
            IDictionary<string, Reminder> reminders = new Dictionary<string, Reminder>();
            foreach (var item in remindersDB)
            {
                Reminder temp = item.Object;
                if (temp.UsernameTo == userTo)
                {
                    reminders.Add(item.Key, temp);
                }
            }
            return reminders;
        }

        //adds reminder
        //private async Task<bool> AddReminderLocallyAsync()
        //{

        //    IDictionary<int, Reminder> reminders = await GetRemindersAsync();
        //    reminder.Id = reminders.Count;
        //    //add the file
        //    reminders.Add(reminder.Id, reminder);
        //    string jsonFormat = JsonConvert.SerializeObject(reminders);
        //    await File.WriteAllTextAsync(filePath, jsonFormat);
        //    return (reminders.Count > 0);
        //}


        //public async Task<IDictionary<int, Reminder>> GetRemindersAsync()
        //{
        //    //create file if doesnt exists
        //    if (!File.Exists(filePath))
        //    {
        //        File.WriteAllText(filePath, "");
        //    }
        //    //read file
        //    string jsonFormat = await File.ReadAllTextAsync(filePath);

        //    IDictionary<int, Reminder> reminders = null;

        //    //empty file than return new object or else convert
        //    if (string.IsNullOrEmpty(jsonFormat))
        //    {
        //        reminders = new Dictionary<int, Reminder>();
        //    }
        //    else
        //    {
        //        reminders = JsonConvert.DeserializeObject<IDictionary<int, Reminder>>(jsonFormat);
        //    }
        //    return reminders;
        //}
        ////filter the reminders depending on users [locally]
        //public async Task<IDictionary<int, Reminder>> GetRemindersForUserAsync(string userTo)
        //{
        //    if (userTo == null) { return null; }

        //    IDictionary<int, Reminder> reminders = await GetRemindersAsync();
        //    IDictionary<int, Reminder> remindersForUser = new Dictionary<int, Reminder>();

        //    foreach (var reminderInfo in reminders)
        //    {
        //        Reminder reminderDetails = reminderInfo.Value;
        //        if (reminderDetails.UsernameTo == userTo)
        //        {
        //            remindersForUser.Add(reminderInfo.Key, reminderDetails);
        //        }
        //    }

        //    return remindersForUser;
        //}

    }
}
