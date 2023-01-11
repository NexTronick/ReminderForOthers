using ReminderForOthers.View;
using ReminderForOthers.ViewModel;

namespace ReminderForOthers;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(SignUp), typeof(SignUp));
		Routing.RegisterRoute(nameof(SignUpNext), typeof(SignUpNext));
        Routing.RegisterRoute(nameof(Login), typeof(Login));
		Routing.RegisterRoute(nameof(PersonalReminders), typeof(PersonalReminders));
    }
}
