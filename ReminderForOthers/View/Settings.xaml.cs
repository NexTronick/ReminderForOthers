using ReminderForOthers.ViewModel;
namespace ReminderForOthers.View;


public partial class Settings : ContentPage
{
	public Settings()
	{
		InitializeComponent();
		BindingContext = new SettingsViewModel();
        App.Window.Stopped += (s, e) =>
        {
            BindingContext = new SettingsViewModel();
        };
    }
	protected override void OnAppearing()
	{
		base.OnAppearing();
        BindingContext = new SettingsViewModel();
  //      ((SettingsViewModel)BindingContext).SetCurrentUserCommand.Execute(this);
		//((SettingsViewModel)BindingContext).SetInitialValuesCommand.Execute(this);
  //      ((SettingsViewModel)BindingContext).SetInitialValuesCommand.Execute(this);
       
    }
}