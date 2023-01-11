

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ReminderForOthers.Model
{
    public class LoginModel
    {
        //variables
        private SignUpModel signUpModel;
        private readonly string LogFile = "UserLog.txt";
        private string logPath;

        //constructor
        public LoginModel()
        {
            signUpModel = new SignUpModel();
            logPath = Path.Combine(FileSystem.Current.CacheDirectory, LogFile);
        }

        public async Task<int> ValidateUserLogin(string username, string password)
        {

            IDictionary<string, User> usersDict = await signUpModel.GetUsersLocally();
            int valid = await signUpModel.ValidatePasswordAsync(username, password);
            if (valid == 1) { await StoreLogInCacheAsync(username); }
            return valid;//login failed no user exits
        }
        //to get or store login cache [locally]
        public async Task<string> GetLogInCacheAsync() 
        {
            if (!File.Exists(logPath)) { return null; }
            return await File.ReadAllTextAsync(logPath);
        }

        private async Task StoreLogInCacheAsync(string username) 
        {
            await File.WriteAllTextAsync(logPath, username);
        }

        //logs gets deleted
        public void Logout() 
        {
            if (!File.Exists(logPath)) { return; }
            File.Delete(logPath);
        }
    }
}
