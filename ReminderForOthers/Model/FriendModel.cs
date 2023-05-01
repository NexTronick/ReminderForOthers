using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using FirebaseAdmin;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ReminderForOthers.View;


namespace ReminderForOthers.Model
{
    public class FriendRequest
    {
        //user sending request
        public string Username { get; set; }
        //sent to user
        public string FriendUsername { get; set; }
        //accept [0: Neutral, 1: Accepted, -1: Declined]
        public int Accept { get; set; }
        //Request Creation Date
        public DateTime RequestDate { get; set; }
    }


    internal class FriendModel
    {
        private const string Web_API_KEY = "AIzaSyDzaUbGPZSIoOknBEOLNOA5KbVGstQ1Sks";
        private const string Database_URL = "https://reminderforothers-default-rtdb.asia-southeast1.firebasedatabase.app";
        private FirebaseClient client = new FirebaseClient(Database_URL);

        //return [1: default [everything is okay], 0: current user sent request already, -1: You already received request, -2: error ]
        public async Task<int> SentRequestAsync(FriendRequest friendRequest)
        {
            try
            {

                //check if firends already
                //[1: default [everything is okay], 0: current user sent request already, -1: You already received request ]
                int isFriendsAlready = 1; 

                //check if friendRequest exits
                var friendsRequests = await client.Child(nameof(FriendRequest)).OnceAsync<FriendRequest>();
                foreach (var item in friendsRequests)
                {
                    //Console.WriteLine("item key: "+item.Key);
                    FriendRequest temp = item.Object;
                    if (temp.Username == friendRequest.Username && temp.FriendUsername == friendRequest.FriendUsername && temp.Accept == 0)
                    {
                        isFriendsAlready = 0; //current user already sent request to friend user
                    }
                    else if (temp.Username == friendRequest.FriendUsername && temp.FriendUsername == friendRequest.Username && temp.Accept == 0)
                    {
                        isFriendsAlready = -1; //other user already sent u request
                    }
                }

                if (isFriendsAlready != 1) { return isFriendsAlready; }

                //not friends then add as FriendRequest
                var response = await client.Child(nameof(FriendRequest)).PostAsync(friendRequest);
                //Console.WriteLine("response Key: "+response.Key);
                return isFriendsAlready;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error Occured", "Friend cannot be set due to: \n" + ex.Message, "Okay");
            }
            return -2;
        }


        //request async
        public async Task<IDictionary<string, FriendRequest>> GetFriendRequestDictionaryAsync(string username)
        {
            try
            {
                var friendsRequests = await client.Child(nameof(FriendRequest)).OnceAsync<FriendRequest>();
                IDictionary<string, FriendRequest> myFriendsRequest = new Dictionary<string, FriendRequest>();
                foreach (var item in friendsRequests)
                {
                   // Console.WriteLine("item key: " + item.Key);
                    FriendRequest temp = item.Object;
                    if (temp.FriendUsername == username && temp.Accept == 0)
                    {
                        myFriendsRequest.Add(item.Key, temp);
                    }
                }
                return myFriendsRequest;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Dictionary<string, FriendRequest>();
            }
        }

        //get friend list async
        public async Task<IDictionary<string, FriendRequest>> GetFriendDictionaryAsync(string username)
        {
            try
            {
                var friends = await client.Child(nameof(Friend)).OnceAsync<FriendRequest>();
                IDictionary<string, FriendRequest> myFriends = new Dictionary<string, FriendRequest>();
                foreach (var item in friends)
                {
                    FriendRequest temp = item.Object;
                    if (temp.FriendUsername == username || temp.Username == username)
                    {
                        myFriends.Add(item.Key, temp);
                    }
                }
                return myFriends;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Dictionary<string, FriendRequest>();
            }

        }

        //convert to friend list
        public List<FriendRequest> ConvertToListFriendRequestObj(string currentUsername, IDictionary<string,FriendRequest> freindsDic) 
        {
            List<FriendRequest> friendList = new List<FriendRequest>();
            foreach (var item in freindsDic)
            {
                FriendRequest tempRequest = item.Value;
                if (tempRequest.FriendUsername == currentUsername)
                {
                    string tempUsername = item.Value.Username;
                    tempRequest.Username = currentUsername;
                    tempRequest.FriendUsername = tempUsername;
                }
                //Console.WriteLine("Friend Username: "+tempRequest.FriendUsername);
                friendList.Add(tempRequest);
            }
            return friendList;
        }

        //accept freind request database
        public async Task<bool> AcceptFriendRequestAsync(string key, FriendRequest friendRequest)
        {
            try
            {
                await client.Child(nameof(FriendRequest)).Child(key).DeleteAsync();
                var response = await client.Child(nameof(Friend)).PostAsync(friendRequest);
                Console.WriteLine("response Key: " + response.Key);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<bool> DeleteFriendRequestAsync(string key, FriendRequest friendRequest)
        {

            try
            {
                //await client.Child(nameof(FriendRequest)).Child(key).PutAsync(friendRequest);
                await client.Child(nameof(FriendRequest)).Child(key).DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> RemoveFriendAsync(string key, FriendRequest friend)
        {
            try
            {
                await client.Child(nameof(Friend)).Child(key).DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

    }
}
