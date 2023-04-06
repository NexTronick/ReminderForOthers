
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReminderForOthers.View;
using ReminderForOthers.Model;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ReminderForOthers.ViewModel
{
    public partial class PersonalReminderViewModel : ObservableObject,INotifyPropertyChanged
    {
        [ObservableProperty]
        bool isReminderSentRefreshed;
        [ObservableProperty]
        bool isReminderReceivedRefreshed;

        public ObservableCollection<Reminder> ObserveReminders { get; set; } = new ObservableCollection<Reminder>();
        public ObservableCollection<Reminder> ObserveSentReminders { get; set; } = new ObservableCollection<Reminder>();
        public event PropertyChangedEventHandler PropertyChanged;

        private MainViewModel mainViewModel;
        private ReminderModel reminderModel;
        private RecordModel recordModel;


        public PersonalReminderViewModel()
        {
            mainViewModel = new MainViewModel();
            reminderModel = new ReminderModel();
            recordModel = new RecordModel();
            LoadData(); 
            LoadSentData();
        }

        public async void LoadData()
        {
            //load reminders have received
            List<Reminder> reminders = await GetRemindersAsync();
            ObserveReminders = new ObservableCollection<Reminder>(reminders);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ObserveReminders)));
        }
        public async void LoadSentData() 
        {
            //load reminders that were sent
            List<Reminder> reminderSent = await GetRemindersSentAsync();
            ObserveSentReminders = new ObservableCollection<Reminder>(reminderSent);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ObserveSentReminders)));
        }

        //gets reminders sent to friend user
        public async Task<List<Reminder>> GetRemindersSentAsync()
        {

            List<Reminder> remindersSent = await reminderModel.GetSentRemindersAsync(await mainViewModel.GetUserLoggedInAsync());
            if (remindersSent == null) { return new List<Reminder>(); }

            Reminder[] arrangedReminders = RearrangeDictionary(remindersSent.ToArray());

            return arrangedReminders.ToList();
        }

        //gets reminders to current user
        public async Task<List<Reminder>> GetRemindersAsync()
        {

            List<Reminder> currentUserReminders = await reminderModel.GetReceivedRemindersAsync(await mainViewModel.GetUserLoggedInAsync());
            if (currentUserReminders == null) { return new List<Reminder>(); }

            Reminder[] arrangedReminders = RearrangeDictionary(currentUserReminders.ToArray());

            return arrangedReminders.ToList();
        }

        //helper method to re arrange according to date and time
        private Reminder[] RearrangeDictionary(Reminder[] reminders)
        {
            for (int i = 0; i < reminders.Length; i++)
            {
                for (int j = reminders.Length - 1; j > i; j--)
                {
                    //Console.WriteLine($"Before switch, Reminder i:{reminders[i].Id}, Reminder j: {reminders[j].Id}");
                    if (reminders[i].PlayDateTime >= reminders[j].PlayDateTime)
                    {
                        Reminder temp = reminders[i];
                        reminders[i] = reminders[j];
                        reminders[j] = temp;
                        Console.WriteLine($"After switch, Reminder i:{reminders[i].Id}, Reminder j: {reminders[j].Id}");
                    }
                }
            }
            return reminders;
        }

        //refresh view
        [RelayCommand]
        void RefreshReceivedReminders()
        {
            LoadData();
            isReminderReceivedRefreshed = true;
        }

        [RelayCommand]
        void RefreshSentReminders() 
        {
            LoadSentData();
            isReminderSentRefreshed = true;
        }

        [RelayCommand]
        async void PlayReminderAsync(string recordPath) 
        {
            Console.WriteLine("RecordPath: "+recordPath);
            string filePath = await reminderModel.GetAudioFilePathAsync(recordPath);
            await recordModel.PlayDownloadedAudio(filePath);
        }

        [RelayCommand]
        public async Task LogoutUserAsync()
        {
            await Shell.Current.GoToAsync("..");
            await mainViewModel.LogoutUserAsync();
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
