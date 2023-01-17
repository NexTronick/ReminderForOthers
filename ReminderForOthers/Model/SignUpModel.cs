using Newtonsoft.Json;
using Microsoft.Maui.Storage;

using System.Net.Mail;


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
    }



    public class SignUpModel
    {
        //variables
        User user;

        

        private readonly string FILENAME = "Users.txt";
        //local directory
        private string mainDir;
        IDictionary<string, User> usersLocal;


        //cloud directory

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
            user.Password = password;

            mainDir = Path.Combine(FileSystem.Current.AppDataDirectory, FILENAME);
            usersLocal = new Dictionary<string, User>();
        }

        //store the file into the local database.
        //returns 3 status, -1 email exists, 0 username exists, 1 user created
        public async Task<int> StoreUserAsync()
        {
            //store local

            //setting local user
            usersLocal = await GetUsersLocallyAsync();
            int storeUser = await StoreUserLocalAsync();


            //store cloud

            //print current users
            //foreach (var userInfo in usersLocal)
            //{
            //    string username = userInfo.Key;
            //    User userDetails = userInfo.Value;
            //    Console.WriteLine($"User: {username}");
            //    Console.WriteLine($"Details: {userDetails.LastName}, {userDetails.FirstName}, {userDetails.BirthDate} ,{userDetails.Username},{userDetails.Email},{userDetails.Password}");
            //}
            return storeUser;

        }

        //store user in the local device [1 doesnt exists,0 username, -1 email]
        private async Task<int> StoreUserLocalAsync()
        {
            //read the file
            //if file doesnt exits then create file
            if (!File.Exists(mainDir))
            {
                await File.WriteAllTextAsync(mainDir, "");
            }

            //check if user already exists locally
            int userExists = DoesUserExistsAsync(user.Username, user.Email);
            if (userExists < 1) { return userExists; } //username or email exists

            //add user locally
            usersLocal.Add(user.Username, user);
            string jsonUsers = JsonConvert.SerializeObject(usersLocal);
            await File.WriteAllTextAsync(mainDir, jsonUsers);
            return userExists;
        }

        public async Task<IDictionary<string, User>> GetUsersLocallyAsync()
        {
            if (!File.Exists(mainDir)) { File.WriteAllText(mainDir, ""); }
            string content = await File.ReadAllTextAsync(mainDir);
            if (string.IsNullOrEmpty(content))
            {
                return new Dictionary<string, User>();
            }
            return JsonConvert.DeserializeObject<IDictionary<string, User>>(content);
        }

        //checks if user exits [1 doesnt exists,0 username, -1 email]
        private int DoesUserExistsAsync(string username, string email)
        {
            //username exists
            if (usersLocal.ContainsKey(username)) { return 0; }
            foreach (var userInfo in usersLocal)
            {
                User userDetails = userInfo.Value;
                //email exists
                if (userDetails.Email == email)
                {
                    return -1;
                }
            }
            return 1; //doesnt exists

        }

        //true or false (validation)
        public async Task<bool> DoesUserNameExitsAsync(string username)
        {

            IDictionary<string, User> userDict = await GetUsersLocallyAsync();
            return userDict.ContainsKey(username); //false (doesnt exists)
        }

        //returns [-1 username doesnt exits, 0 password wrong, 1 password correct ] 
        public async Task<int> ValidatePasswordAsync(string username, string password)
        {
            IDictionary<string, User> userDict = await GetUsersLocallyAsync();
            if (!userDict.ContainsKey(username)) { return -1; }
            userDict.TryGetValue(username, out User userInfo);
            if (userInfo.Password.Equals(password)) { return 1; }
            return 0;
        }

        public void FireBaseStoreDB()
        {

        }

    }
}
