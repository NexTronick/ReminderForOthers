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
}