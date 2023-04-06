using Plugin.LocalNotification;
using ReminderForOthers.Model;
using ReminderForOthers.ViewModel;
namespace ReminderForOthers.View;

public partial class PersonalReminders : ContentPage
{
	public PersonalReminders()
	{
		InitializeComponent();
		BindingContext = new PersonalReminderViewModel();
        //Shell.SetNavBarIsVisible(this, false);
        //SetNotificationsAsync();
    }
    protected async void SetNotificationsAsync()
    {
        List<Reminder> reminders = await ((PersonalReminderViewModel)BindingContext).GetRemindersAsync();
        if (reminders == null) { return; }

        foreach (var reminder in reminders)
        {
            var request = new NotificationRequest
            {
                NotificationId = 100 + reminder.Id,
                Title = reminder.Title,
                Subtitle = "Reminder",
                Description = $"Remember to {reminder.Title}, from {reminder.UsernameFrom}",
                BadgeNumber = 42,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = reminder.PlayDateTime,
                    NotifyRepeatInterval = TimeSpan.FromDays(1)
                }
            };
            Console.WriteLine($"Time: {reminder.PlayDateTime}");
            await LocalNotificationCenter.Current.Show(request);

        }
    }
}