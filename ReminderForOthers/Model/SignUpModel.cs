using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
        private string jsonUser;
        private readonly string FILENAME = "Users.json";
        //constructor
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
            IDictionary<string, User> userDict = new Dictionary<string, User>();
            userDict.Add(user.Username, user);
            jsonUser = JsonConvert.SerializeObject(userDict);
            string mainDir = FileSystem.Current.AppDataDirectory;
            mainDir = Path.Combine(mainDir, FILENAME);
            var outputStream = File.OpenWrite(mainDir);
            var streamWriter = new StreamWriter(outputStream);
            streamWriter.Write(jsonUser);

            Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(mainDir);
            StreamReader reader = new StreamReader(fileStream);

            string content = await reader.ReadToEndAsync();

            IDictionary<string, User> userDictCheck = JsonConvert.DeserializeObject<IDictionary<string, User>>(content);

            foreach (var userInfo in userDictCheck)
            {
                string username = userInfo.Key;
                User userDetails = userInfo.Value;
                Console.WriteLine($"User: {username}");
                Console.WriteLine($"Details: {userDetails.LastName}, {userDetails.FirstName}, {userDetails.BirthDate} ,{userDetails.Username},{userDetails.Email},{userDetails.Password}");
            }

            Console.WriteLine(jsonUser);
        }

        private async Task AppendUserLocally()
        {
            string mainDir = FileSystem.Current.AppDataDirectory;

            // Write the file content to the app data directory
            //string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, targetFileName);

            //FileStream outputStream = System.IO.File.OpenWrite(targetFile);
            //StreamWriter streamWriter = new StreamWriter(outputStream);

            //await streamWriter.WriteAsync(content);
        }

        //public async Task<IDictionary<string, User>> GetLocalUsers()
        //{
        //    // Read the source file
        //    IDictionary<string, User> userDict = null;
        //    try
        //    {
        //        string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, FILENAME);
        //        Console.WriteLine($"Path: {targetFile}");
        //        bool fileExists = await FileSystem.Current.AppPackageFileExistsAsync(targetFile);
        //        Console.WriteLine($"File Exists: {fileExists}");
        //        if (!fileExists)
        //        {
        //            FileStream outputStream = File.OpenWrite(targetFile);
        //            StreamWriter streamWriter = new StreamWriter(outputStream);

        //            await streamWriter.WriteAsync("");
        //        }
        //        fileExists = await FileSystem.Current.AppPackageFileExistsAsync(targetFile);
        //        Console.WriteLine($"File Exists: {fileExists}");
        //        //Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(mainDir);
        //        //StreamReader reader = new StreamReader(fileStream);

        //        //string content = await reader.ReadToEndAsync();

        //        //userDict = JsonConvert.DeserializeObject<IDictionary<string, User>>(content);

        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine($"Error in GetLocalUsers: {e}");
        //    }
        //    return userDict;

        //    //string username = "";

        //    //foreach (var userInfo in userDict)
        //    //{
        //    //    string username = userInfo.Key;
        //    //    User userDetails = userInfo.Value;
        //    //    Console.WriteLine($"User: {username}");
        //    //    Console.WriteLine($"Details: {userDetails.LastName}, {userDetails.FirstName}, {userDetails.BirthDate} ,{userDetails.Username},{userDetails.Email},{userDetails.Password}");
        //    //}

        //}
        public void FireBaseStoreDB()
        {

        }

    }
}
