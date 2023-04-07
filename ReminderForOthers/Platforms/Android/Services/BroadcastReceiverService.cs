using Android.App;
using Android.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndroidApp = Android.App.Application;

namespace ReminderForOthers.Platforms.Android.Services
{
    [BroadcastReceiver(Enabled =true, Exported =true)]
    [IntentFilter(new[] {Intent.ActionBootCompleted })]
    public class BroadcastReceiverService : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if(intent.Action == Intent.ActionBootCompleted)
            {
                var foregroundServiceIntent = new Intent(AndroidApp.Context, typeof(ForegroundService));
                AndroidApp.Context.StartForegroundService(intent);
                context.StartForegroundService(foregroundServiceIntent);
            }
        }
    }
}
