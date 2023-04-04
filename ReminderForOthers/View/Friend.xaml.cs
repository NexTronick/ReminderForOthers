using ReminderForOthers.ViewModel;

namespace ReminderForOthers.View;

public partial class Friend : ContentPage
{
	public Friend()
	{
		InitializeComponent();
        BindingContext = new FriendViewModel();
        //Shell.SetNavBarIsVisible(this, false);
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        BindingContext = new FriendViewModel();

    }
}