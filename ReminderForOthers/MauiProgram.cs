using Microsoft.Extensions.DependencyInjection;
using ReminderForOthers.ViewModel;
using ReminderForOthers.View;
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

        builder.Services.AddTransient<Login>();
        builder.Services.AddTransient<LoginViewModel>();

        builder.Services.AddTransient<SignUp>();
        builder.Services.AddTransient<SignUpNext>();
        builder.Services.AddSingleton<SignUpViewModel>();

        return builder.Build();
	}
}
