using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReminderForOthers.View;

namespace ReminderForOthers.ViewModel
{
    public partial class FriendViewModel : ObservableObject
    {
        private string username;

        [RelayCommand]
        async void AddFriend()
        {
            username = await Shell.Current.DisplayPromptAsync("Add Friend", "Write down the username of a friend.", keyboard: Keyboard.Text);
            await Shell.Current.DisplayAlert("Friend Request Sent", "User '" + username + "' will have to accept the freind request in order to become your friend.", "Okay");
        }

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
            await Shell.Current.GoToAsync("./" + nameof(MainPage)); //home set to be Personal Reminders
        }

        [RelayCommand]
        async void GotoFriends()
        {
            await Shell.Current.GoToAsync(nameof(Friend)); //home set to be Personal Reminders
        }

        [RelayCommand]
        async void GoBack() => await Shell.Current.GoToAsync("..");

        [RelayCommand]
        async void GotoNotification() => await Shell.Current.GoToAsync("..");
    }
}
