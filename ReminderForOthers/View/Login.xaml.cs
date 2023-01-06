using ReminderForOthers.ViewModel;

namespace ReminderForOthers;

public partial class Login : ContentPage
{
	public Login()
	{
		InitializeComponent();
        BindingContext = new LoginViewModel();
    }

	private void TapGestureRecognizer_SignUp(object sender, EventArgs e)
	{
        ((LoginViewModel)BindingContext).SignUpNowCommand.Execute(this);


    }
}