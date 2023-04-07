
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
//using ReminderForOthers.Platforms.Android.Services;
using ReminderForOthers.Services;
using System.ComponentModel;

namespace ReminderForOthers.ViewModel
{
    public partial class SettingsViewModel : ObservableObject, INotifyPropertyChanged
    {

        [ObservableProperty]
        string currentUser;

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _foregroundChecked;
        public bool ForegroundChecked
        {
            get => _foregroundChecked;
            set
            {
                if (_foregroundChecked != value)
                {
                    ToggleBackgroundNotification(value);
                }
            }
        }

        private MainViewModel mainViewModel;

        public SettingsViewModel()
        {
            mainViewModel = new MainViewModel();
            SetCurrentUser();
        }

        private async void SetCurrentUser()
        {
            currentUser = await mainViewModel.GetUserLoggedInAsync();
        }



        [RelayCommand]
        void CheckForeground()
        {
            ToggleBackgroundNotification(!_foregroundChecked);
        }
        private void ToggleBackgroundNotification(bool check)
        {
            _foregroundChecked = check;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ForegroundChecked)));

            //toggle services
            ToggleForegroundService(check);
        }
        //to turn the service on and off
        private void ToggleForegroundService(bool check)
        {
            if (check && !DependencyService.Resolve<IForegroundService>().IsForegroundServiceRunning())
            {
                DependencyService.Resolve<IForegroundService>().Start();
                Shell.Current.DisplayAlert("Foreground Service Started", "The background service is running.", "Okay");
            }
            else if(!check && DependencyService.Resolve<IForegroundService>().IsForegroundServiceRunning())
            {
                DependencyService.Resolve<IForegroundService>().Stop();
                Shell.Current.DisplayAlert("Foreground Service Stopped", "The background service has stopped.", "Okay");
            }

        }

        [RelayCommand]
        public async Task LogoutUserAsync()
        {
            //await Shell.Current.GoToAsync("..");
            await mainViewModel.LogoutUserAsync();
        }
    }
}
