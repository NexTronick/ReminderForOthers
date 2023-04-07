using Plugin.LocalNotification;
using ReminderForOthers.Model;
using ReminderForOthers.ViewModel;

namespace ReminderForOthers.Platforms.Android.Services
{
    public class ReminderNotificationService
    {
        private static RecordModel recordModel = new RecordModel();
        private static PersonalReminderViewModel personalReminderViewModel = new PersonalReminderViewModel();

        private List<Reminder> reminders = new List<Reminder>();
        private List<Reminder> reminded = new List<Reminder>();

        private static bool reminderStarted;

        public void RunReminderServices(int getIntervalmSec, int playIntervalmSec)
        {

            RunGetReminderService(getIntervalmSec);
            RunPlayReminderService(playIntervalmSec);
        }

        private void RunGetReminderService(int intervalSec)
        {
            Task.Run(() =>
            {
                while (reminderStarted)
                {
                    UpdateReminders();
                    Thread.Sleep(intervalSec);
                }
            });
        }

        private async void UpdateReminders()
        {
            List<Reminder> tempReminders = await personalReminderViewModel.GetRemindersAsync();
            if (reminders.Count == tempReminders.Count) { return; }
            reminders = tempReminders;
            //SetNotificationsAsync();
        }

        private void RunPlayReminderService(int intervalSec)
        {
            Task.Run(() =>
            {
                while (reminderStarted)
                {
                    PlayNotification();
                    System.Diagnostics.Debug.WriteLine("Run Play Reminder Service is Running");
                    Thread.Sleep(intervalSec);
                }
            });
        }



        private async void SetNotificationsAsync(int i, Reminder reminder)
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
            reminded.Add(reminder);
        }

        private void PlayNotification()
        {
            for (int i = 0; i <= reminders.Count; i++)
            {
                Reminder tempReminder = reminders[i];
                if (reminded.Contains(tempReminder)) { continue; } //if user has already reminded

                int tenSeconds = 10;
                TimeSpan reminderTime = new TimeSpan(tempReminder.PlayDateTime.Ticks);
                TimeSpan currentTime = new TimeSpan(DateTime.Now.Ticks);

                if (reminderTime.TotalSeconds - tenSeconds <= currentTime.TotalSeconds)
                {
                    //make a reminder task and Notification
                    SetNotificationsAsync(i, tempReminder);
                    Task.Run(() =>
                    {
                        Thread.Sleep(1000*tenSeconds);
                        personalReminderViewModel.PlayReminderAsync(tempReminder.RecordPath);

                    });
                    //remove from database


                    //update the current reminders

                }
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
