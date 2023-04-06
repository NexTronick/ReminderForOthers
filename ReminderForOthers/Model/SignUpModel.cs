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

        //constructors
        public SignUpModel()
        {
            mainDir = Path.Combine(FileSystem.Current.AppDataDirectory, FILENAME);
            usersLocal = new Dictionary<string, User>();
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
                int userExists = 1;
                await Task.Delay(1000);
                foreach (var item in usersList)
                {
                    User tempUser = item.Value;
                    Console.WriteLine($"{tempUser.Username} : {user.Username}");
                    if (tempUser.Username == user.Username) { userExists = 0; }
                    else if (tempUser.Email == user.Email) { userExists = -1; }
                }

                

                Console.WriteLine($"User Exists: {userExists}");
                //if user exists
                if (userExists < 1) { return userExists; }
                //add user
                await client.Child("Users").PostAsync<User>(user);
                return userExists;

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
                FirebaseClient client = new FirebaseClient(Database_URL);

                var users = await client.Child("Users").OnceAsync<User>();

                IDictionary<string, User> usersList = new Dictionary<string, User>();
                foreach (var item in users)
                {
                    User user = (User)item.Object;
                    usersList.Add(user.Username, user);
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
