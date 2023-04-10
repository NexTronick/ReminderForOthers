using Plugin.LocalNotification;
using ReminderForOthers.Model;
using ReminderForOthers.ViewModel;
namespace ReminderForOthers.View;

public partial class PersonalReminders : ContentPage
{
	public PersonalReminders()
	{
		InitializeComponent();
		BindingContext = new PersonalReminderViewModel();
        //Shell.SetNavBarIsVisible(this, false);
    }
	protected override void OnAppearing()
	{
		base.OnAppearing();
        BindingContext = new PersonalReminderViewModel();
    }

}