using ReminderForOthers.ViewModel;
namespace ReminderForOthers;

public partial class SignUp : ContentPage
{
	public SignUp(SignUpViewModel signUpVM)
	{
		InitializeComponent();
		BindingContext = signUpVM;
        Shell.SetNavBarIsVisible(this, false);
    }

	private void TapGestureRecognizer_HasAccount(object sender, EventArgs e)
	{
        ((SignUpViewModel)BindingContext).HasAccountFromSignUpCommand.Execute(this);
    }
}