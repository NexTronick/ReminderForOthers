

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
        private string tempPath;

        //constructor
        public LoginModel()
        {
            signUpModel = new SignUpModel();
            logPath = Path.Combine(FileSystem.Current.CacheDirectory, LogFile);
            tempPath = Path.Combine(FileSystem.Current.CacheDirectory, "UserTemp.txt");
        }

        public async Task<int> ValidateUserLogin(string username, string password, bool checkBox)
        {
            int valid = await signUpModel.ValidatePasswordAsync(username, password);
            if (valid == 1 && checkBox) { await StoreLogInCacheAsync(logPath, username); }
            if (valid == 1) { await StoreLogInCacheAsync(tempPath, username); }
            return valid;//login failed no user exits
        }

        //to get or store login cache [locally]
        public async Task<string> GetLogInCacheAsync()
        {
            if (File.Exists(logPath))
            {
                return await File.ReadAllTextAsync(logPath);
            }
            else if (!File.Exists(logPath) && File.Exists(tempPath))
            {
                return await File.ReadAllTextAsync(tempPath);
            }
            return "";
        }

        public async Task<string> GetStartLoginInfoAsync() 
        {
            if (File.Exists(logPath))
            {
                return await File.ReadAllTextAsync(logPath);
            }
            return "";
        }

        private async Task StoreLogInCacheAsync(string path, string username)
        {
            await File.WriteAllTextAsync(path, username);
        }

        //logs gets deleted
        public void Logout()
        {
            if (File.Exists(tempPath)) { File.Delete(tempPath); } //remove temp path
            if (!File.Exists(logPath)) { return; }
            File.Delete(logPath);
        }
    }
}
