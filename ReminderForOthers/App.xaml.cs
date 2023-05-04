using ReminderForOthers.Model;
using ReminderForOthers.Services;
using ReminderForOthers.View;
namespace ReminderForOthers;

public partial class App : Application
{
    private LoginModel loginModel;
    private SettingsModel settingsModel;
	public App()
	{
		InitializeComponent();
		MainPage = new AppShell();
        loginModel = new LoginModel();
        settingsModel = new SettingsModel();
        CheckUserLoggedIn();
        
    }
    private async void CheckUserLoggedIn()
    {
        string username = await loginModel.GetStartLoginInfoAsync();
        if (string.IsNullOrEmpty(username))
        {
            //Shell.SetTabBarIsVisible(this, loggedIn);
            await Shell.Current.GoToAsync("//Login");
        }
        else {
            await Shell.Current.GoToAsync("//Home");
            LoadSettingsServices();
        }
    }
    private async void LoadSettingsServices() 
    {
        await settingsModel.LoadSettings();
    }
    public static Window Window { get; private set; }
    protected override Window CreateWindow(IActivationState activationState)
    {
        Window window = base.CreateWindow(activationState);
        Window = window;
        window.Stopped += (s, e) =>
        {
            //System.Diagnostics.Debug.WriteLine("=========Stopped");
            SettingsChangedWithStop(true);
        };

        return window;
    }

    private async void SettingsChangedWithStop(bool isStopped) 
    {
        //if user has saved its data then exit code
        string username = await loginModel.GetStartLoginInfoAsync();
        if (!string.IsNullOrEmpty(username)) { return; }

        //if foreground service isnt on exit code
        SettingsService settingsService = await settingsModel.ReadSettings();
        if (isStopped && !settingsService.ForegroundServiceOn) { return; }

        //toggle forground service for future
        settingsService.ForegroundServiceOn = !isStopped;
        await settingsModel.WriteSettings(settingsService);
        await settingsModel.LoadSettings();
    }
}
