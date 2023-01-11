
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
            //for (int i = 0; i < 10; i++)
            //{
            //    reminders.Add(new Reminder { Id = i, Date = DateTime.Now, Time = DateTime.Now.TimeOfDay, Title = "Brush Teeth", RecordPath = "d/d/d/d", ReminderCreationTime = DateTime.Now, UsernameFrom = "johnChan", UsernameTo = "nat" });
            //}
            
            ObserveReminders = new ObservableCollection<Reminder>(reminders);
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(ObserveReminders)));
        }

        [RelayCommand]
        public async Task LogoutUserAsync() 
        {
            await Shell.Current.GoToAsync("..");
            await mainViewModel.LogoutUserAsync();
        }
    }
}
