using ReminderForOthers.ViewModel;

namespace ReminderForOthers.View;

public partial class SignUpNext : ContentPage
{
	public SignUpNext(SignUpViewModel signUpVM)
	{
		InitializeComponent();
		BindingContext = signUpVM;
        Shell.SetNavBarIsVisible(this, false);
    }

    private void TapGestureRecognizer_HasAccount(object sender, EventArgs e)
    {
        ((SignUpViewModel)BindingContext).HasAccountFromSignUpNextCommand.Execute(this);
    }
}