
using Newtonsoft.Json;

namespace ReminderForOthers.Model
{
    public class Reminder
    {
        public int Id { get; set; }
        public string UsernameFrom { get; set; }
        public string UsernameTo { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string RecordPath { get; set; }
        public DateTime ReminderCreationTime { get; set; }
    }
    public class ReminderModel
    {
        //recordings
        private Reminder reminder;
        private readonly string fileName = "Reminders.txt";

        public string filePath { get; }

        public ReminderModel() {
            filePath = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);
        }

        public ReminderModel(string usernameFrom, string usernameTo, string title, DateTime date, TimeSpan time, string recordPath)
        {
            //set all the values for reminders
            reminder = new Reminder();
            reminder.UsernameFrom = usernameFrom;
            reminder.UsernameTo = usernameTo;
            reminder.Title = title;
            reminder.Date = date;
            reminder.Time = time;
            reminder.RecordPath = recordPath;
            reminder.ReminderCreationTime = DateTime.Now;

            filePath = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);

        }

        public async Task<bool> StoreReminderAsync()
        {
            //add into data
            bool storedFlag = await AddReminderLocallyAsync();
            return storedFlag;
        }

        //adds reminder
        private async Task<bool> AddReminderLocallyAsync()
        {

            IDictionary<int, Reminder> reminders = await GetRemindersAsync();
            reminder.Id = reminders.Count;
            //add the file
            reminders.Add(reminder.Id, reminder);
            string jsonFormat = JsonConvert.SerializeObject(reminders);
            await File.WriteAllTextAsync(filePath, jsonFormat);
            return (reminders.Count > 0);
        }


        public async Task<IDictionary<int, Reminder>> GetRemindersAsync()
        {
            //create file if doesnt exists
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "");
            }
            //read file
            string jsonFormat = await File.ReadAllTextAsync(filePath);

            IDictionary<int, Reminder> reminders = null;

            //empty file than return new object or else convert
            if (string.IsNullOrEmpty(jsonFormat))
            {
                reminders = new Dictionary<int, Reminder>();
            }
            else
            {
                reminders = JsonConvert.DeserializeObject<IDictionary<int, Reminder>>(jsonFormat);
            }
            return reminders;
        }
        //filter the reminders depending on users [locally]
        public async Task<IDictionary<int, Reminder>> GetRemindersForUserAsync(string userTo) 
        {
            if (userTo == null) { return null; }

            IDictionary<int, Reminder> reminders = await GetRemindersAsync();
            IDictionary<int, Reminder> remindersForUser = new Dictionary<int, Reminder>();

            foreach (var reminderInfo in reminders) 
            {
                Reminder reminderDetails = reminderInfo.Value;
                if (reminderDetails.UsernameTo == userTo) 
                {
                    remindersForUser.Add(reminderInfo.Key, reminderDetails);
                }
            }

            return remindersForUser;
        }

    }
}
