using ReminderForOthers.ViewModel;
using Plugin.LocalNotification;
using ReminderForOthers.Model;
using ReminderForOthers.View;

namespace ReminderForOthers;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainViewModel();
        //Shell.SetNavBarIsVisible(this, false);
        PermissionsModel permissionsModel = new PermissionsModel();
        Task<bool> run = permissionsModel.AskRequiredPermissionsAsync();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        BindingContext = new MainViewModel();
    }

    

}

