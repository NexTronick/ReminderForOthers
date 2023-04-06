
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ReminderForOthers.ViewModel
{
    public partial class SettingsViewModel : ObservableObject
    {

        [ObservableProperty]
        string currentUser;

        private MainViewModel mainViewModel;
       
        public SettingsViewModel()
        {
            mainViewModel = new MainViewModel();
            SetCurrentUser();
        }

        private async void SetCurrentUser() {
            currentUser =  await mainViewModel.GetUserLoggedInAsync();
        }

        [RelayCommand]
        public async Task LogoutUserAsync()
        {
            //await Shell.Current.GoToAsync("..");
            await mainViewModel.LogoutUserAsync();
        }
    }
}
