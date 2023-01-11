using Microsoft.Extensions.DependencyInjection;
using ReminderForOthers.ViewModel;
using ReminderForOthers.View;
using ReminderForOthers.Model;

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
		
		
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<MainViewModel>();
		builder.Services.AddTransient<PermissionsModel>();
        builder.Services.AddTransient<RecordModel>();
		builder.Services.AddTransient<ReminderModel>();

        builder.Services.AddTransient<PersonalReminders>();
        builder.Services.AddTransient<PersonalReminderViewModel>();

        builder.Services.AddTransient<Login>();
        builder.Services.AddTransient<LoginViewModel>();

        builder.Services.AddTransient<SignUp>();
        builder.Services.AddTransient<SignUpNext>();
        builder.Services.AddSingleton<SignUpViewModel>();
		builder.Services.AddTransient<SignUpModel>();

        return builder.Build();
	}
}
