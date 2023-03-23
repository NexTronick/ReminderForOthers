using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReminderForOthers.Model;
using ReminderForOthers.View;

namespace ReminderForOthers.ViewModel
{
    public partial class FriendViewModel : ObservableObject
    {
        private string username;
        private FriendModel friendModel;
        private LoginModel loginModel;
        public FriendViewModel() { 
            friendModel = new FriendModel();
            loginModel = new LoginModel();

        }

        [RelayCommand]
        async void AddFriend()
        {
            string placeHolder = "eg. John";
            username = await Shell.Current.DisplayPromptAsync("Add Friend", "Write down the username of a friend.", keyboard: Keyboard.Text, placeholder: placeHolder);

            //if they click cancel
            if (username == null) { return; }

            if (string.IsNullOrEmpty(username))
            {
                string[] btn = { "Yes", "No" };
                bool val = await Shell.Current.DisplayAlert("Username Empty", "Username is empty, would you like to add again?", "Yes", "No");
                Console.WriteLine("DisplayAlert: " + val);

                if (!val) { return; }
                AddFriend();
                return; //to exit the code if it runs again.
            }

            SendFriendRequest(username);
        }

        //to add friends onto database for request
        private async void SendFriendRequest(string username)
        {
            try
            {
                
                FriendRequest friendRequest = new FriendRequest();
                friendRequest.Username = await loginModel.GetLogInCacheAsync(); //gets logged in user info
                friendRequest.FriendUsername = username;
                friendRequest.Accept = 0;
                friendRequest.RequestDate = DateTime.Now;

                if (friendRequest.Username == username) {
                    await Shell.Current.DisplayAlert("Friend Request Failed", "User \"" + username + "\" is your username.", "Okay");
                    return;
                }

                bool response = await friendModel.SentRequestAsync(friendRequest);

                if (!response)
                {
                    await Shell.Current.DisplayAlert("Friend Request Failed", "User \"" + username + "\" does not exists.", "Okay");
                    return;
                }

                Console.WriteLine("Time RN: " + DateTime.Now);
                Console.WriteLine("Response: " + response);

                await Shell.Current.DisplayAlert("Friend Request Sent", "User \"" + username + "\" will have to accept the freind request in order to become your friend.", "Okay");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Friend Request Error", "User \"" + username + "\" friend request could not be sent.\n" + ex.Message, "Okay");

            }
        }

        async Task<List<FriendRequest>> GetFriendRequestsAsync() 
        {
            return await friendModel.GetFriendRequestAsync(await loginModel.GetLogInCacheAsync());

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
