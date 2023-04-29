using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReminderForOthers.Model;
//using ReminderForOthers.Platforms.Android.Services;
using ReminderForOthers.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ReminderForOthers.ViewModel
{
    public partial class SettingsViewModel : ObservableObject, INotifyPropertyChanged
    {

        [ObservableProperty]
        string currentUser;

        public User User { get; set; }
        private string userKey;
        private User initialUser;

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
        private SignUpModel signUpModel;
        private SettingsModel settingsModel;
        public SettingsViewModel()
        {
            loginModel = new LoginModel();
            signUpModel = new SignUpModel();
            settingsModel = new SettingsModel();
            SetCurrentUser();
            SetInitialSettings();
        }

        public async void SetInitialSettings()
        {
            SettingsService settingsService = await settingsModel.ReadSettings();
            ForegroundChecked = settingsService.ForegroundServiceOn;
        }

        [RelayCommand]
        public async void SetCurrentUser()
        {
            CurrentUser = await loginModel.GetLogInCacheAsync();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentUser)));

            User = await signUpModel.GetUserFromUsernameAsync(CurrentUser);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(User)));
            initialUser = await signUpModel.GetUserFromUsernameAsync(CurrentUser);
            userKey = await signUpModel.GetUserKeyAsync(User);
            //Console.WriteLine("Current User: "+ CurrentUser);
        }

        [RelayCommand]
        public async void SaveSettingChanges()
        {

            SaveSettingService();
            if (!await AreDetailsChangedValid()) { return; }
            bool isNewUsername = User.Username != initialUser.Username;
            bool success = await signUpModel.UpdateUserInfoAsync(userKey, User, isNewUsername);

            //failed
            if (!success)
            {
                await Shell.Current.DisplayAlert("Account Settings", "Account Settings Failed to update!\nEither Username or Email are already taken.", "Okay");
                return;
            }

            //success

            //different username
            if (User.Username != initialUser.Username)
            {
                await Shell.Current.DisplayAlert("Account Settings", "Account Settings are Updated!\nFrom Username '" + initialUser.Username + "' to '" + User.Username + "'\nDirecting back to Login.", "Okay");
                await LogoutUserAsync();
                return;
            }

            //update the new user info
            SetCurrentUser();
            await Shell.Current.DisplayAlert("Account Settings", "Account Settings are Updated!", "Okay");
        }

        private async void SaveSettingService()
        {
            SettingsService settingsService = await settingsModel.ReadSettings();
            if (settingsService.ForegroundServiceOn != _foregroundChecked)
            {
                settingsService.ForegroundServiceOn = _foregroundChecked;
                await settingsModel.WriteSettings(settingsService);
            }
        }

        private async Task<bool> AreDetailsChangedValid()
        {
            //details are changed
            if (initialUser.Username != User.Username ||
                initialUser.Email != User.Email ||
                initialUser.FirstName != User.FirstName ||
                initialUser.LastName != User.LastName ||
                initialUser.BirthDate != User.BirthDate)
            {
                SignUpViewModel signUpViewModel = new SignUpViewModel();
                return signUpViewModel.ValidateUsersDetails(User); //validate
            }

            //if no details changed
            return false;
        }

        [RelayCommand]
        void CheckForeground()
        {
            ToggleBackgroundNotification(!_foregroundChecked);
            SaveSettingService();
        }
        private void ToggleBackgroundNotification(bool check)
        {
            _foregroundChecked = check;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ForegroundChecked)));

            //save services
            SaveSettingService();

            //Start Service
            settingsModel.SetForegroundService(check);//to turn the service on and off
        }


        [RelayCommand]
        public async Task LogoutUserAsync()
        {
            loginModel.Logout();
            settingsModel.DeleteSettings();
            SetInitialSettings();
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
