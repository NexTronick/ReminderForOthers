using ReminderForOthers.Model;
using ReminderForOthers.ViewModel;

namespace ReminderForOthers;

public partial class Login : ContentPage
{
    public Login()
    {
        InitializeComponent();
        BindingContext = new LoginViewModel();
        Shell.SetNavBarIsVisible(this, false);
        PermissionsModel permissionsModel = new PermissionsModel();
        Task<bool> run = permissionsModel.AskRequiredPermissionsAsync();
    }
    private void TapGestureRecognizer_SignUp(object sender, EventArgs e)
	{
        ((LoginViewModel)BindingContext).SignUpNowCommand.Execute(this);
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        BindingContext = new LoginViewModel();
        
    }

}