using ReminderForOthers.ViewModel;
namespace ReminderForOthers.View;

public partial class Friend : ContentPage
{
	public Friend()
	{
		InitializeComponent();
		BindingContext = new FriendViewModel();
		Shell.SetNavBarIsVisible(this,false);
	}
}