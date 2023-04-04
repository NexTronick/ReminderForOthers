
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReminderForOthers.View;
using ReminderForOthers.Model;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ReminderForOthers.ViewModel
{
    public partial class PersonalReminderViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Reminder> ObserveReminders { get; set; } = new ObservableCollection<Reminder>();

        private List<Reminder> reminders = new List<Reminder>();
        private MainViewModel mainViewModel;

        public event PropertyChangedEventHandler PropertyChanged;

        public PersonalReminderViewModel() 
        {
            mainViewModel = new MainViewModel();
            LoadData();
        }

        public async void LoadData() 
        {
            reminders = await mainViewModel.GetRemindersAsync();
            ObserveReminders = new ObservableCollection<Reminder>(reminders);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ObserveReminders)));
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
