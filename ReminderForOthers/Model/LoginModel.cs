

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
        public bool isLoginSaved;

        //constructor
        public LoginModel()
        {
            signUpModel = new SignUpModel();
            logPath = Path.Combine(FileSystem.Current.CacheDirectory, LogFile);
            tempPath = Path.Combine(FileSystem.Current.CacheDirectory, "UserTemp.txt");
            SetIsLoginSaved();
        }

        public async Task<int> ValidateUserLogin(string username, string password, bool checkBox)
        {
            int valid = await signUpModel.ValidatePasswordAsync(username, password);
            if (valid == 1 && checkBox) { await StoreLogInCacheAsync(logPath, username); }
            if (valid == 1)
            {
                string tempPath = Path.Combine(FileSystem.Current.CacheDirectory, "UserTemp.txt");
                await StoreLogInCacheAsync(tempPath, username);
            }
            return valid;//login failed no user exits
        }
        private void SetIsLoginSaved()
        {
            if (!File.Exists(logPath))
            {
                isLoginSaved = false;
            }
            else
            {
                isLoginSaved = true;
            }
        }

        //to get or store login cache [locally]
        public async Task<string> GetLogInCacheAsync()
        {
            if (!isLoginSaved && !File.Exists(tempPath))
            {
                return null;
            }
            else if (!isLoginSaved && File.Exists(tempPath))
            {
                return await File.ReadAllTextAsync(tempPath);
            }
            return await File.ReadAllTextAsync(logPath);
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
