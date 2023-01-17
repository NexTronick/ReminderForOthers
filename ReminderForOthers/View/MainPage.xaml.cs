using ReminderForOthers.ViewModel;
using Plugin.LocalNotification;
using ReminderForOthers.Model;

namespace ReminderForOthers;

public partial class MainPage : ContentPage
{

    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainViewModel();
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
        Task remind = SetNotificationsAsync();

    }

    protected async Task SetNotificationsAsync()
    {
        List<Reminder> reminders = await ((MainViewModel)BindingContext).GetRemindersAsync();
        if (reminders == null) { return;  }

        foreach (var reminder in reminders)
        {
            var request = new NotificationRequest
            {
                NotificationId = 100+reminder.Id,
                Title = reminder.Title,
                Subtitle = "Reminder",
                Description = $"Remember to {reminder.Title}, from {reminder.UsernameFrom}",
                BadgeNumber = 42,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = reminder.Date.AddTicks(reminder.Time.Ticks),
                    NotifyRepeatInterval = TimeSpan.FromDays(1)
                }
            };
            Console.WriteLine($"Time: {reminder.Date.AddTicks(reminder.Time.Ticks)}");
            await LocalNotificationCenter.Current.Show(request);
            
        }
    }
 
}

