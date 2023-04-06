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
    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        Console.WriteLine("Binding context changed");
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Console.WriteLine("OnAppearing");
        Task login = ((MainViewModel)BindingContext).GotoLoginPageCommand.ExecuteAsync(this);
        Console.WriteLine("Finished Appearing On MainPage");
        //SetNotificationsAsync();
        //Task move = Shell.Current.GoToAsync(nameof(Friend));

    }

    

}

