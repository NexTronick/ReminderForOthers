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
        //constructors
        public SignUpModel() { }

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
        }

        //store the file into the local database.
        public async Task StoreUser()
        {
            //store local
            await StoreUserLocal();

            //store cloud


            IDictionary<string, User> userDict = await GetUsersLocally();

            foreach (var userInfo in userDict)
            {
                string username = userInfo.Key;
                User userDetails = userInfo.Value;
                Console.WriteLine($"User: {username}");
                Console.WriteLine($"Details: {userDetails.LastName}, {userDetails.FirstName}, {userDetails.BirthDate} ,{userDetails.Username},{userDetails.Email},{userDetails.Password}");
            }

        }

        //to get main directory of file
        public string GetFullPath()
        {
            return Path.Combine(FileSystem.Current.AppDataDirectory, FILENAME);
        }

        //store user in the local device
        private async Task StoreUserLocal()
        {
            //read the file
            string mainDir = GetFullPath();
            //File.Delete(mainDir);
            //if file doesnt exits then create file
            if (!File.Exists(mainDir))
            {
                await File.WriteAllTextAsync(mainDir, "");
            }

            //add user
            await AddUserLocally();
        }

        //add users locally 
        private async Task AddUserLocally()
        {
            IDictionary<string, User> users = await GetUsersLocally();
            users.Add(user.Username, user);
            string jsonUsers = JsonConvert.SerializeObject(users);
            await File.WriteAllTextAsync(GetFullPath(), jsonUsers);
        }

        public async Task<IDictionary<string, User>> GetUsersLocally()
        {
            string mainDir = GetFullPath();
            if (!File.Exists(mainDir)) { File.WriteAllText(mainDir, ""); }
            string content = await File.ReadAllTextAsync(mainDir);
            if (string.IsNullOrEmpty(content))
            {
                return new Dictionary<string, User>();
            }
            return JsonConvert.DeserializeObject<IDictionary<string, User>>(content);
        }

        //true or false (validation)
        public async Task<bool> DoesUserExitsAsync(string username)
        {

            IDictionary<string, User> userDict = await GetUsersLocally();
            return userDict.ContainsKey(username); ; //false (doesnt exists)
        }
        public async Task<int> ValidatePasswordAsync(string username, string password)
        {   
            IDictionary<string, User> userDict = await GetUsersLocally();
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
