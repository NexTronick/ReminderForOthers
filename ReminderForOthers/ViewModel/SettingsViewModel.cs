
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
            Console.WriteLine("Check: " + check);
            _foregroundChecked = check;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ForegroundChecked)));

            //foregroundChecked = !foregroundChecked;
            //if (foregroundChecked)
            //{
            //    Console.WriteLine("Foregorund Service is On");
            //}
            //else
            //{
            //    Console.WriteLine("Foregorund Service is Off");
            //}
        }

        [RelayCommand]
        public async Task LogoutUserAsync()
        {
            //await Shell.Current.GoToAsync("..");
            await mainViewModel.LogoutUserAsync();
        }
    }
}
