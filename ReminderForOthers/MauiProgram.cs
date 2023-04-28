using Microsoft.Extensions.DependencyInjection;
using ReminderForOthers.ViewModel;
using ReminderForOthers.View;
using ReminderForOthers.Model;
using Microsoft.Maui.LifecycleEvents;
using FirebaseAdmin;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Plugin.Maui.Audio;
using Plugin.LocalNotification;
//using Plugin.Firebase.Auth;
//#if IOS
//using Plugin.Firebase.Core.Platforms.iOS;
//#elif ANDROID
//using Plugin.Firebase.Core.Platforms.Android;
//#endif
namespace ReminderForOthers;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		//adding all the services we are using
		builder.Services.AddSingleton(AudioManager.Current);

		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<MainViewModel>();
		builder.Services.AddTransient<PermissionsModel>();
        builder.Services.AddTransient<RecordModel>();
		builder.Services.AddTransient<ReminderModel>();


        builder.Services.AddTransient<PersonalReminderViewModel>();
        builder.Services.AddTransient<PersonalReminders>();
        

        builder.Services.AddTransient<Login>();
        builder.Services.AddTransient<LoginModel>();
        builder.Services.AddTransient<LoginViewModel>();


        builder.Services.AddTransient<SignUp>();
        builder.Services.AddTransient<SignUpNext>();
        builder.Services.AddSingleton<SignUpViewModel>();
		builder.Services.AddSingleton<SignUpSingleton>();
		builder.Services.AddTransient<SignUpModel>();

        builder.Services.AddTransient<Friend>();
        builder.Services.AddTransient<FriendModel>();
        builder.Services.AddTransient<FriendViewModel>();

		builder.Services.AddTransient<Settings>();
        builder.Services.AddTransient<SettingsViewModel>();

        return builder.Build();
	}
//    private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
//    {
//        builder.ConfigureLifecycleEvents(events => {
//#if IOS
//            events.AddiOS(iOS => iOS.FinishedLaunching((_,__) => {
//                CrossFirebase.Initialize();
//                return false;
//            }));
//#elif ANDROID
//            events.AddAndroid(android => android.OnCreate((activity, _) =>
//                CrossFirebase.Initialize(activity)));
//#endif
//        });

//        builder.Services.AddSingleton(_ => CrossFirebaseAuth.Current);
//        return builder;
//    }
}
