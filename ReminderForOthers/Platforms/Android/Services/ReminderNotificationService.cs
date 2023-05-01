﻿using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using ReminderForOthers.Model;
using ReminderForOthers.Services;
using ReminderForOthers.ViewModel;
using System.Linq;
using static Android.Content.ClipData;

namespace ReminderForOthers.Platforms.Android.Services
{
    public class ReminderNotificationService
    {
        private static RecordModel recordModel = new RecordModel();
        private static LoginModel loginModel = new LoginModel();
        private static ReminderModel reminderModel = new ReminderModel();
        private static AudioPlayerService audioService = new AudioPlayerService();

        private IDictionary<string, Reminder> reminders = new Dictionary<string, Reminder>();
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
                    GetUpdatedReminders();

                    Thread.Sleep(intervalSec);
                }
            });
        }

        private void GetUpdatedReminders()
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
                        System.Diagnostics.Debug.WriteLine("Play Audio  Error: " + ex.Message);
                    }

                }
            });
        }


        private void PlayNotificationAsync()
        {
            System.Diagnostics.Debug.WriteLine("PlayNotificationAsync started");
            foreach (var item in reminders)
            {
                System.Diagnostics.Debug.WriteLine("Running Loop Audio");
                Reminder reTemp = item.Value;
                if (reminded.Contains(item)) { continue; } //if user has already reminded

                int tenSeconds = 10;
                TimeSpan reminderTime = new TimeSpan(reTemp.PlayDateTime.Ticks);
                TimeSpan currentTime = new TimeSpan(DateTime.Now.Ticks);

                if (reminderTime.TotalSeconds - tenSeconds <= currentTime.TotalSeconds && !reminderIsPlaying && !reTemp.HasPlayed)
                {
                    PlayReminder(item.Key, reTemp);
                }
                else if (reminderTime.TotalSeconds - tenSeconds <= currentTime.TotalSeconds && !reminderIsPlaying && reTemp.HasPlayed)
                {
                    RemoveReminder(item.Key, reTemp);
                }
            }


        }

        //remove reminder after a day
        private async void RemoveReminder(string key, Reminder reminder) 
        {
            if (reminder.PlayDateTime.AddDays(1) > DateTime.Now) 
            {
                return;
            }
            await reminderModel.RemoveReminderFirestore(key, reminder);
            GetUpdatedReminders();
        }

        //helper method to play reminder
        private void PlayReminder(string key, Reminder reminder)
        {
            //make a reminder task and Notification
            NotificationModel.NotifyReminders(key, reminder);

            //Play Notification
            reminderIsPlaying = true;
            int totalSleep = NotificationModel.SchedulePlay(key, reminder);
            Thread.Sleep(totalSleep);

            //update the current reminders
            reminder.HasPlayed = true;
            reminderModel.UpdateReminderFirestore(key, reminder).Wait();

            //get new updated reminders
            GetUpdatedReminders();
            reminderIsPlaying = false;
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
