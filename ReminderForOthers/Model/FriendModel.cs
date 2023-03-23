using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using FirebaseAdmin;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ReminderForOthers.View;
using static Android.Gms.Common.Apis.Api;

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

        public async Task<bool> SentRequestAsync(FriendRequest friendRequest)
        {
            try
            {
               
                //check if friend username exists
                SignUpModel signUpModel = new SignUpModel();
                bool exists = await signUpModel.DoesUserNameExits(friendRequest.FriendUsername);
                if (!exists) { return false; }

                //check if firends already
                int isFriendsAlready = -2; //[0: current user sent request already, 1: already friends, -1: You already received request, -2: default ]

                var friends = await client.Child(nameof(Friend)).OnceAsync<FriendRequest>();
                foreach (var item in friends)
                {
                    Console.WriteLine("item key: "+item.Key);
                    FriendRequest temp = item.Object;
                    if (temp.Username == friendRequest.Username && temp.FriendUsername == friendRequest.FriendUsername)
                    {
                        if (temp.Accept == 1) { isFriendsAlready = 1; }
                        else if (temp.Accept == 0) { isFriendsAlready = 0; }
                    }
                    else if (temp.Username == friendRequest.FriendUsername && temp.FriendUsername == friendRequest.Username)
                    {
                        if (temp.Accept == 1) { isFriendsAlready = 1; }
                        else if (temp.Accept == 0) { isFriendsAlready = -1; } //other user already sent u request
                    }
                }

                if (isFriendsAlready != -2) { return false; }

                //not friends then add as friends
                var response =  await client.Child(nameof(FriendRequest)).PostAsync(friendRequest);
                Console.WriteLine("response Key: "+response.Key);
                return true;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error Occured", "Friend cannot be set due to: \n" + ex.Message, "Okay");
            }
            return false;
        }

        //private async Task<List<FriendRequest>> GetAllFriendRequestsAsync() {
        //    var friends = await client.Child("Friends").OnceAsync<FriendRequest>();
        //    return friends;
        //}

        public async Task<IDictionary<string,FriendRequest>> GetFriendRequestAsync(string username)
        {
            
            var friends = await client.Child("Friends").OnceAsync<FriendRequest>();
            IDictionary<string, FriendRequest> myFriends = new Dictionary<string,FriendRequest>();
            foreach (var item in friends)
            {
               FriendRequest temp  =  item.Object;
                if (temp.FriendUsername == username && temp.Accept == 0) {
                    myFriends.Add(item.Key,temp);
                } 
            }
            return myFriends;
        }
        //public async Task<bool> AcceptFriendRequestAsync(FriendRequest friendRequest) 
        //{

        //    FirebaseClient client = new FirebaseClient(Database_URL);

        //}
    }
}
