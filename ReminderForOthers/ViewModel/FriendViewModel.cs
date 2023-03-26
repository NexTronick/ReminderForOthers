
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FirebaseAdmin.Auth.Multitenancy;
using ReminderForOthers.Model;
using ReminderForOthers.View;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ReminderForOthers.ViewModel
{
    public partial class FriendViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<FriendRequest> ObserveFriendRequests { get; set; } = new ObservableCollection<FriendRequest>();
        public ObservableCollection<FriendRequest> ObserveFriendList { get; set; } = new ObservableCollection<FriendRequest>();

        private string username;
        private FriendModel friendModel;
        private LoginModel loginModel;

        private IDictionary<string, FriendRequest> friendRequests;
        private IDictionary<string, FriendRequest> friends;

        public FriendViewModel()
        {
            friendModel = new FriendModel();
            loginModel = new LoginModel();
            friendRequests = new Dictionary<string, FriendRequest>();
            friends = new Dictionary<string, FriendRequest>();
            LoadFreindRequestsAsync();
            LoadFriendListAsync();

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

        //helper method checks if friend exits 
        private async Task<bool> DoesFriendExist(FriendRequest request)
        {
            //current user

            foreach (var item in friends)
            {
                if (item.Value.FriendUsername == request.FriendUsername)
                {
                    await Shell.Current.DisplayAlert("Friend Request Failed", "User \"" + request.FriendUsername + "\" is your Friend already.", "Okay");
                    return true;
                }
            }

            return false;
        }

        //helper method to add friends onto database for request
        private async void SendFriendRequest(string friendUsername)
        {
            try
            {
                //check if friend username exists
                SignUpModel signUpModel = new SignUpModel();
                bool userExists = await signUpModel.DoesUserNameExits(friendUsername);
                if (!userExists)
                {
                    await Shell.Current.DisplayAlert("Friend Request Failed", "User \"" + friendUsername + "\" does not exists.", "Okay");
                    return;
                }

                //check if username is current username
                string currentUsername = await loginModel.GetLogInCacheAsync();
                if (currentUsername == friendUsername)
                {
                    await Shell.Current.DisplayAlert("Friend Request Failed", "User \"" + friendUsername + "\" is your username.", "Okay");
                    return;
                }

                //create request
                FriendRequest friendRequest = new FriendRequest();
                friendRequest.Username = await loginModel.GetLogInCacheAsync(); //gets logged in user info
                friendRequest.FriendUsername = friendUsername;
                friendRequest.Accept = 0;
                friendRequest.RequestDate = DateTime.Now;

                //checks if friend exists
                bool friendExits = await DoesFriendExist(friendRequest);
                if (friendExits) { return; }

                //add user now
                int response = await friendModel.SentRequestAsync(friendRequest);

                switch (response)
                {
                    case 1:
                        await Shell.Current.DisplayAlert("Friend Request Sent", "User \"" + friendUsername + "\" will have to accept the freind request in order to become your friend.", "Okay");
                        break;
                    case 0:
                        //current user sent request already
                        await Shell.Current.DisplayAlert("Friend Request Failed", "You have already sent friend request to \"" + friendUsername + "\". You will have to wait for the user to accept the request.", "Okay");
                        break;
                    case -1:
                        //request is sent by friend user
                        await Shell.Current.DisplayAlert("Friend Request Failed", "You have received Friend Request from \"" + friendUsername + "\". Please check in Friend Requests to Accept.", "Okay");
                        await Shell.Current.DisplayAlert("Friend Request Accept/Decline", "Note: To get Options of Accept/Decline on a Friend Request Swipe from Left to Right on the Selected Friend.", "Okay");
                        break;
                    case -2:
                        await Shell.Current.DisplayAlert("Friend Request Failed", "Internal Error, please try again later.", "Okay");
                        break;
                }

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Friend Request Error", "User \"" + friendUsername + "\" friend request could not be sent.\n" + ex.Message, "Okay");

            }
        }

        //load the freind requests
        [RelayCommand]
        public async void LoadFreindRequestsAsync()
        {
            string currentUsername = await loginModel.GetLogInCacheAsync();
            friendRequests = await friendModel.GetFriendRequestAsync(currentUsername);
            List<FriendRequest> requests = new List<FriendRequest>();
            foreach (var item in friendRequests)
            {
                FriendRequest tempRequest = item.Value;
                if (tempRequest.FriendUsername == currentUsername)
                {
                    string tempUsername = item.Value.Username;
                    tempRequest.Username = currentUsername;
                    tempRequest.FriendUsername = tempUsername;
                }
                requests.Add(tempRequest);
            }
            ObserveFriendRequests = new ObservableCollection<FriendRequest>(requests);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ObserveFriendRequests)));
        }

        //accept friend request
        [RelayCommand]
        public async void AcceptRequest(FriendRequest request)
        {
            string key = FindRequestedKey(request, friendRequests);
            if (key == "") { return; }
            ObserveFriendRequests.Remove(request);
            request.Accept = 1;
            await friendModel.AcceptFriendRequestAsync(key, request);
            friendRequests.Remove(key);
            ObserveFriendList.Add(request);
        }

        [RelayCommand]
        public async void DeleteRequest(FriendRequest request)
        {
            string key = FindRequestedKey(request, friendRequests);
            if (key == "") { return; }
            ObserveFriendRequests.Remove(request);
            request.Accept = -1;
            await friendModel.DeleteFriendRequestAsync(key, request);
            friendRequests.Remove(key);
        }


        //load friend list
        [RelayCommand]
        public async void LoadFriendListAsync()
        {
            string currentUsername = await loginModel.GetLogInCacheAsync();
            friends = await friendModel.GetFriendListAsync(currentUsername);
            List<FriendRequest> friendList = new List<FriendRequest>();
            foreach (var item in friends)
            {
                FriendRequest tempRequest = item.Value;
                if (tempRequest.FriendUsername == currentUsername)
                {
                    string tempUsername = item.Value.Username;
                    tempRequest.Username = currentUsername;
                    tempRequest.FriendUsername = tempUsername;
                }
                friendList.Add(tempRequest);
            }
            ObserveFriendList = new ObservableCollection<FriendRequest>(friendList);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ObserveFriendList)));
        }

        //remove friend
        [RelayCommand]
        public async void RemoveFriend(FriendRequest friend)
        {
            string key = FindRequestedKey(friend, friends);
            if (key == "") { return; }
            ObserveFriendList.Remove(friend);
            await friendModel.RemoveFriendAsync(key, friend);
            friends.Remove(key);
        }


        //helper method to get key from list
        private string FindRequestedKey(FriendRequest request, IDictionary<string, FriendRequest> list)
        {
            foreach (var item in list)
            {
                if (item.Value == request)
                {
                    return item.Key;
                }
            }
            return "";
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
            //await Shell.Current.GoToAsync("./" + nameof(MainPage)); //home set to be Personal Reminders
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
