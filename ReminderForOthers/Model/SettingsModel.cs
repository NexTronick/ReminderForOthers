using Newtonsoft.Json;
using ReminderForOthers.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ReminderForOthers.Model
{
    public class SettingsService
    {
        public bool ForegroundServiceOn { set; get; }
    }
    public class SettingsModel
    {
        private readonly string SettingName = "Settings.txt";
        private string settingsPath;

        //constructor
        public SettingsModel()
        {
            settingsPath = Path.Combine(FileSystem.Current.CacheDirectory, SettingName);
        }

        public async Task<bool> WriteSettings(SettingsService settings)
        {
            try
            {
                string output = JsonConvert.SerializeObject(settings, Formatting.Indented);
                await File.WriteAllTextAsync(settingsPath, output);
                return true;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            return false;
        }
        public async Task<SettingsService> ReadSettings()
        {
            if (!File.Exists(settingsPath)) { return new SettingsService(); }
            string file = await File.ReadAllTextAsync(settingsPath);
            return JsonConvert.DeserializeObject<SettingsService>(file);
        }

        public async Task<bool> LoadSettings() 
        {
            try
            {
                SettingsService settingsService = await ReadSettings();
                SetForegroundService(settingsService.ForegroundServiceOn);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
        public async void SetForegroundService(bool check) 
        {
            if (check && !DependencyService.Resolve<IForegroundService>().IsForegroundServiceRunning())
            {
                DependencyService.Resolve<IForegroundService>().Start();
                await Shell.Current.DisplayAlert("Foreground Service Started", "The background service is running.", "Okay");
            }
            else if (!check && DependencyService.Resolve<IForegroundService>().IsForegroundServiceRunning())
            {
                DependencyService.Resolve<IForegroundService>().Stop();
                await Shell.Current.DisplayAlert("Foreground Service Stopped", "The background service has stopped.", "Okay");
            }

        }
    }
}
