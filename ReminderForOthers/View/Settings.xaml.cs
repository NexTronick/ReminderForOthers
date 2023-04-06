using ReminderForOthers.ViewModel;
namespace ReminderForOthers.View;


public partial class Settings : ContentPage
{
	public Settings()
	{
		InitializeComponent();
		BindingContext = new SettingsViewModel();
    }
}