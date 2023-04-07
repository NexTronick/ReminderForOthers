using Android.App;
using Android.Runtime;
using ReminderForOthers.Platforms.Android.Services;
using ReminderForOthers.Services;

namespace ReminderForOthers;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
        DependencyService.Register<IForegroundService, ForegroundService>();
    }

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
