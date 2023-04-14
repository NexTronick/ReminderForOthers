
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReminderForOthers.Model;
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

        private LoginModel loginModel;
        public SettingsViewModel()
        {
            loginModel = new LoginModel();
            SetCurrentUser();
        }

        [RelayCommand]
        public async void SetCurrentUser()
        {
            CurrentUser = await loginModel.GetLogInCacheAsync();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentUser)));
            Console.WriteLine("Current User: "+ CurrentUser);
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
            loginModel.Logout();
            await Shell.Current.GoToAsync("//Login");
        }

        //public async Task GotoLoginPageAsync()
        //{
        //    //move to login page
        //    if (string.IsNullOrEmpty(currentUser))
        //    {
        //        await Shell.Current.GoToAsync("//Login");
        //    }
        //    //await Shell.Current.GoToAsync(nameof(Login));
        //}

        
    }
}
