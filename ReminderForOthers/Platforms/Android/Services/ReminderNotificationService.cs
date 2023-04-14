using Plugin.LocalNotification;
using ReminderForOthers.Model;
using ReminderForOthers.Services;
using ReminderForOthers.ViewModel;
using System.Linq;

namespace ReminderForOthers.Platforms.Android.Services
{
    public class ReminderNotificationService
    {
        private static RecordModel recordModel = new RecordModel();
        private static LoginModel loginModel = new LoginModel();
        private static ReminderModel reminderModel = new ReminderModel();
        private static AudioPlayerService audioService = new AudioPlayerService();

        private IDictionary<string,Reminder> reminders = new Dictionary<string, Reminder>();
        private IDictionary<string, Reminder> reminded = new Dictionary<string, Reminder>();

        private static bool reminderStarted;
        private static bool reminderIsPlaying;

        public void RunReminderServices(int getIntervalmSec, int playIntervalmSec)
        {
            StartService();
            RunGetReminderService(getIntervalmSec);
            RunPlayReminderService(playIntervalmSec);
        }

        private void RunGetReminderService(int intervalSec)
        {
            Task.Run(() =>
            {
                while (reminderStarted)
                {
                    System.Diagnostics.Debug.WriteLine("Run Get Reminder Service is Running");
                    UpdateReminders();
                    
                    Thread.Sleep(intervalSec);
                }
            });
        }

        private void UpdateReminders()
        {
            System.Diagnostics.Debug.WriteLine("UpdateReminders started");
           
            Task<IDictionary<string, Reminder>> tempReminders = reminderModel.GetReceivedRemindersAsync(loginModel.GetLogInCacheAsync().Result);
            if (reminders.Count == tempReminders.Result.Count) { return; }
            reminders = tempReminders.Result;
            System.Diagnostics.Debug.WriteLine("Result");
            //SetNotificationsAsync();
        }

        private void RunPlayReminderService(int intervalSec)
        {
            Task.Run(() =>
            {
                while (reminderStarted)
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine("Run Play Reminder Service is Running");
                        PlayNotificationAsync();
                        Thread.Sleep(intervalSec);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Play Audio  Error: "+ex.Message);
                    }
                    
                }
            });
        }

        private async void SetNotificationsAsync(int i,string documentID,Reminder reminder)
        {
            var request = new NotificationRequest
            {
                NotificationId = 1 + i,
                Title = reminder.Title,
                Subtitle = "Reminder",
                Description = $"Remember to {reminder.Title}, from {reminder.UsernameFrom}",
                BadgeNumber = 42,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = reminder.PlayDateTime
                    //NotifyRepeatInterval = TimeSpan.FromDays(1)
                }
            };
            Console.WriteLine($"Time: {reminder.PlayDateTime}");
            await LocalNotificationCenter.Current.Show(request);
            reminded.Add(documentID,reminder);
        }

        private void PlayNotificationAsync()
        {
            int i=0;
            System.Diagnostics.Debug.WriteLine("PlayNotificationAsync started");
            foreach (var item in reminders)
            {
                System.Diagnostics.Debug.WriteLine("Running Loop Audio");
                Reminder reTemp = item.Value;
                if (reminded.Contains(item)) { continue; } //if user has already reminded

                int tenSeconds = 10;
                TimeSpan reminderTime = new TimeSpan(reTemp.PlayDateTime.Ticks);
                TimeSpan currentTime = new TimeSpan(DateTime.Now.Ticks);
                
                if (reminderTime.TotalSeconds - tenSeconds <= currentTime.TotalSeconds && !reminderIsPlaying)
                {
                    //make a reminder task and Notification
                    SetNotificationsAsync(i,item.Key,reTemp);

                    Task<string> filePath = reminderModel.GetAudioFilePathAsync(reTemp.RecordPath);
                    Task.Run(() =>
                    {
                        //need to add a loop where it shows its done or not before opening another thread.
                        reminderIsPlaying = true;
                        Thread.Sleep(1000 * tenSeconds);
                        recordModel.PlayDownloadedAudio(filePath.Result);
                        System.Diagnostics.Debug.WriteLine("Playing Audio");
                        
                    });
                   
                    Thread.Sleep(1000 * tenSeconds + audioService.AudioDuration(filePath.Result));
                    //remove from database
                    Task<bool> removed = reminderModel.RemoveReminderFirestore(item.Key, reTemp);

                    //update the current reminders
                    UpdateReminders();
                    System.Diagnostics.Debug.WriteLine("reminderTime Loop: "+i);
                }
                i++;
            }


        }

        public void StartService()
        {
            reminderStarted = true;
        }
        public void StopService()
        {
            reminderStarted = false;
        }
    }
}
