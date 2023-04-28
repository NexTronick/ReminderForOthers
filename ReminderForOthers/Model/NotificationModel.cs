using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReminderForOthers.Services;

namespace ReminderForOthers.Model
{
    public class NotificationModel
    {
        private static ReminderModel reminderModel = new ReminderModel();
        private static RecordModel recordModel = new RecordModel();
        private static AudioPlayerService audioService = new AudioPlayerService();
        public static async void NotifyReminders(string key, Reminder reminder)
        {
            Random rnd = new Random();
            int notifyId = rnd.Next(1, 1000);
            DateTime notifyTime = reminder.PlayDateTime;
            if (reminder.PlayDateTime < DateTime.Now) 
            {
                notifyTime = DateTime.Now.AddMinutes(1);
            }

            var request = new NotificationRequest
            {
                NotificationId = 11111 + notifyId,
                Title = reminder.Title,
                Subtitle = "Reminder",
                Description = $"Remember to {reminder.Title}, from {reminder.UsernameFrom}",
                BadgeNumber = 42,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = notifyTime
                    //NotifyRepeatInterval = TimeSpan.FromDays(1)
                },
                Android = new AndroidOptions
                {
                    LedColor = 213,
                    VisibilityType = AndroidVisibilityType.Public,
                }
            };
            Console.WriteLine($"Time: {reminder.PlayDateTime}");
            await LocalNotificationCenter.Current.Show(request);
            ScheduleDelete(key, reminder);
        }

        private static void ScheduleDelete(string key, Reminder reminder) {
            Task task = Task.Run(() =>
            {
                TimeSpan timeSpan = DateTime.Now.AddDays(1).Subtract(DateTime.Now);
                Thread.Sleep(Convert.ToInt32(timeSpan.TotalMilliseconds));
                Task<bool> remove = reminderModel.RemoveReminderFirestore(key, reminder);
            });
        }

        public static int SchedulePlay(string key, Reminder reminder) {
            int tenSecondsInMS = 1000 * 10;
            Task<string> filePath = reminderModel.GetAudioFilePathAsync(reminder.RecordPath);
            Task.Run(() =>
            {
                //need to add a loop where it shows its done or not before opening another thread.
                Thread.Sleep(tenSecondsInMS);
                recordModel.PlayDownloadedAudio(filePath.Result);
                System.Diagnostics.Debug.WriteLine("Playing Audio");

            });
            int totalSleep = tenSecondsInMS + audioService.AudioDuration(filePath.Result);
            Thread.Sleep(totalSleep);
            return totalSleep;
        }
    }
}
