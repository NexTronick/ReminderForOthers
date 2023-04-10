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
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        BindingContext = new MainViewModel();
    }

    

}

