using Newtonsoft.Json;
using Microsoft.Maui.Storage;

using System.Net.Mail;
using Firebase.Database;
using Firebase.Auth;
using ReminderForOthers.View;
using Firebase.Database.Query;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;

namespace ReminderForOthers.Model
{
    public class User
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string BirthDate { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreationDate { get; set; }
    }



    public class SignUpModel
    {
        //variables
        private User user;



        private readonly string FILENAME = "Users.txt";
        //local directory
        private string mainDir;
        IDictionary<string, User> usersLocal;


        //cloud directory firebase
        private const string Database_URL = "https://reminderforothers-default-rtdb.asia-southeast1.firebasedatabase.app";
        private FirebaseClient client;
        //constructors
        public SignUpModel()
        {
            mainDir = Path.Combine(FileSystem.Current.AppDataDirectory, FILENAME);
            usersLocal = new Dictionary<string, User>();
            client = new FirebaseClient(Database_URL);
        }

        public SignUpModel(string lName, string fName, string birthDate, string username, MailAddress email, string password)
        {
            //settings all the values for user
            user = new User();
            user.LastName = lName;
            user.FirstName = fName;
            user.BirthDate = birthDate;
            user.Username = username;
            user.Email = email.Address;
            user.Password = ConvertToSHA256(password);
            user.CreationDate = DateTime.Now;

            mainDir = Path.Combine(FileSystem.Current.AppDataDirectory, FILENAME);
            usersLocal = new Dictionary<string, User>();
            client = new FirebaseClient(Database_URL);
        }

        public string ConvertToSHA256(string s)
        {
            string hash = String.Empty;

            // Initialize a SHA256 hash object
            using (SHA256 sha256 = SHA256.Create())
            {
                // Compute the hash of the given string
                byte[] hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(s));

                // Convert the byte array to string format
                foreach (byte b in hashValue)
                {
                    hash += $"{b:X2}";
                }
            }

            return hash;
        }

        //store the file into the local database.
        //returns 3 status, -1 email exists, 0 username exists, 1 user created
        public async Task<int> StoreUserAsync()
        {

            //store cloud
            int storeUser = await StoreUserCloudAsync();

            return storeUser;

        }

        private async Task<int> StoreUserCloudAsync()
        {
            try
            {
                //check if user exits
                FirebaseClient client = new FirebaseClient(Database_URL);
                IDictionary<string, User> usersList = await GetUsers();

                await Task.Delay(1000);
                int userDoesntExist = UserDoesNotExist(user, usersList);

                Console.WriteLine($"User Doesnt Exist: {userDoesntExist}");
                //if user exists
                if (userDoesntExist < 1) { return userDoesntExist; }
                //add user
                await client.Child("Users").PostAsync<User>(user);
                return userDoesntExist;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }
        //Get user dictionary so key is username easy to get data
        private async Task<IDictionary<string, User>> GetUsers()
        {
            try
            {
                var users = await client.Child("Users").OnceAsync<User>();

                IDictionary<string, User> usersList = new Dictionary<string, User>();
                foreach (var item in users)
                {
                    User tempUser = (User)item.Object;
                    usersList.Add(tempUser.Username, tempUser);
                }

                return usersList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        //true or false (validation)
        public async Task<bool> DoesUserNameExits(string username)
        {

            IDictionary<string, User> userDict = await GetUsers();
            return userDict.ContainsKey(username); //false (doesnt exists)
        }
        public int UserDoesNotExist(User checkUser, IDictionary<string, User> usersList)
        {
            int userDoesntExist = 1; //1 means user doesnt exist

            //check for username
            if (usersList.ContainsKey(checkUser.Username))
            {
                userDoesntExist = 0;
                return userDoesntExist;
            }
            //check for email
            foreach (var item in usersList)
            {
                User tempUser = item.Value;
                Console.WriteLine($"{tempUser.Username} : {user.Username}");
                if (tempUser.Email == checkUser.Email) { userDoesntExist = -1; }
            }
            return userDoesntExist;
        }
        public async Task<User> GetUserFromUsernameAsync(string username)
        {
            IDictionary<string, User> userDict = await GetUsers();
            return userDict.TryGetValue(username, out User user) ? user : null;
        }

        //get users with key
        public async Task<string> GetUserKeyAsync(User currentUser)
        {
            try
            {
                var users = await client.Child("Users").OnceAsync<User>();
                foreach (var item in users)
                {
                    User temp = item.Object;
                    if (temp.Username == currentUser.Username)
                    {
                        Console.WriteLine("Item key: " + item.Key);
                        return item.Key;
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return "";
        }

        public async Task<bool> UpdateUserInfoAsync(string key, User user)
        {
            try
            {
                await client.Child("Users").Child(key).PutAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;

        }

        //returns [-1 username doesnt exits, 0 password wrong, 1 password correct ] 
        public async Task<int> ValidatePasswordAsync(string username, string password)
        {
            string hasPass = ConvertToSHA256(password);
            IDictionary<string, User> userDict = await GetUsers();
            if (!userDict.ContainsKey(username)) { return -1; }
            userDict.TryGetValue(username, out User userInfo);
            if (userInfo.Password.Equals(hasPass)) { return 1; }
            return 0;
        }


    }
}
