
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReminderForOthers.View;
using ReminderForOthers.Model;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification;

namespace ReminderForOthers.ViewModel
{
    public partial class PersonalReminderViewModel : ObservableObject
    {
        [ObservableProperty]
        bool isReminderSentRefreshed;
        [ObservableProperty]
        bool isReminderReceivedRefreshed;

        public ObservableCollection<Reminder> ReceviedReminders { get; } = new();
        public ObservableCollection<Reminder> SentReminders { get; } = new();

        private MainViewModel mainViewModel;
        private ReminderModel reminderModel;
        private RecordModel recordModel;
        private LoginModel loginModel;


        private IDictionary<string, Reminder> reminderReceiveDic;
        private IDictionary<string, Reminder> reminderSentDic;
        public PersonalReminderViewModel()
        {
            mainViewModel = new MainViewModel();
            reminderModel = new ReminderModel();
            recordModel = new RecordModel();
            loginModel = new LoginModel();
            Task receivedRemindersTask = RefreshReceivedReminders();
            Task sentRemindersTask = RefreshSentReminders();
        }

        //gets reminders sent to friend user
        private async Task<List<Reminder>> GetRemindersSentAsync()
        {
            reminderSentDic = await reminderModel.GetSentRemindersAsync(await loginModel.GetLogInCacheAsync());
            return reminderModel.ConvertToListReminder(reminderSentDic);
        }

        //gets reminders to current user
        private async Task<List<Reminder>> GetRemindersAsync()
        {
            reminderReceiveDic = await reminderModel.GetReceivedRemindersAsync(await loginModel.GetLogInCacheAsync());
            return reminderModel.ConvertToListReminder(reminderReceiveDic);
        }

        
        //refresh view
        [RelayCommand]
        async Task RefreshReceivedReminders()
        {
            try
            {
                List<Reminder> reminders = await GetRemindersAsync();
                if (reminders.Count == ReceviedReminders.Count)
                {
                    IsReminderReceivedRefreshed = false;
                    return;
                }

                ReceviedReminders.Clear();
                foreach (var reminder in reminders)
                {
                    ReceviedReminders.Add(reminder);
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            IsReminderReceivedRefreshed = false;
        }

        [RelayCommand]
        async Task RefreshSentReminders()
        {
            try
            {
                List<Reminder> reminders = await GetRemindersSentAsync();
                if (reminders.Count == SentReminders.Count)
                {
                    IsReminderSentRefreshed = false;
                    return;
                }

                SentReminders.Clear();
                foreach (var reminder in reminders)
                {
                    SentReminders.Add(reminder);
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            IsReminderSentRefreshed = false;
        }

        [RelayCommand]
        public async void PlayReminderAsync(string recordPath)
        {
            //Console.WriteLine("RecordPath: "+recordPath);
            string filePath = await reminderModel.GetAudioFilePathAsync(recordPath);
            //Console.WriteLine("File duration: "+ReminderAudio.AudioDuration(filePath));
            recordModel.PlayDownloadedAudio(filePath);
        }

        //default navigations

        [RelayCommand]
        async void GotoHome()
        {
            await Shell.Current.GoToAsync(nameof(PersonalReminders)); //home set to be Personal Reminders
        }

        [RelayCommand]
        void GotoSetReminder()
        {
            Shell.SetNavBarIsVisible(Shell.Current.CurrentPage, false); //home set to be Personal Reminders
        }

        [RelayCommand]
        async void GotoSettings()
        {
            await Shell.Current.GoToAsync(nameof(Settings));
        }

        [RelayCommand]
        async void GotoFriends()
        {
            await Shell.Current.GoToAsync(nameof(Friend)); //home set to be Personal Reminders
        }

        [RelayCommand]
        async void GoBack() => await Shell.Current.GoToAsync("..");

        [RelayCommand]
        async void GotoNotification()
        {
            //await Shell.Current.GoToAsync("..");
            await Shell.Current.DisplayAlert("Notification", "Notification is to be added.", "Okay");
        }
    }
}
