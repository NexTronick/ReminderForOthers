using ReminderForOthers.ViewModel;

namespace ReminderForOthers;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
		BindingContext = new MainViewModel();
		Task login = Shell.Current.GoToAsync(nameof(Login));

    }
}

