using ReminderForOthers.Model;
using ReminderForOthers.Services;
using ReminderForOthers.View;
namespace ReminderForOthers;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		MainPage = new AppShell();
        CheckUserLoggedIn();
        LoadSettingsServices();
    }
    private async void CheckUserLoggedIn()
    {
        string LogFile = "UserLog.txt";
        string logPath = Path.Combine(FileSystem.Current.CacheDirectory, LogFile);
        bool loggedIn = File.Exists(logPath);
        if (!loggedIn)
        {
            //Shell.SetTabBarIsVisible(this, loggedIn);
            await Shell.Current.GoToAsync("//Login");
        }
        else {
            await Shell.Current.GoToAsync("//Home");
        }
    }
    private async void LoadSettingsServices() 
    {
        SettingsModel settingsModel = new SettingsModel();
        await settingsModel.LoadSettings();
    }
}
