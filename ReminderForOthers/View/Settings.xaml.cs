using ReminderForOthers.ViewModel;
namespace ReminderForOthers.View;


public partial class Settings : ContentPage
{
	public Settings()
	{
		InitializeComponent();
		BindingContext = new SettingsViewModel();
    }
	protected override void OnAppearing()
	{
		base.OnAppearing();
		((SettingsViewModel)BindingContext).SetCurrentUserCommand.Execute(this);
		((SettingsViewModel)BindingContext).SetInitialValuesCommand.Execute(this);
    }
}