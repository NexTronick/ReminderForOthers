using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using Microsoft.Maui.Controls.PlatformConfiguration;
using ReminderForOthers.Platforms.Android.Services;
using ReminderForOthers.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndroidApp = Android.App.Application;

//[assembly: Dependency(typeof(ForegroundService))]
namespace ReminderForOthers.Platforms.Android.Services
{
    [Service]
    internal class ForegroundService : Service, IForegroundService
    {
        private static bool isForegroundServiceRunning;
        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            Task.Run(() =>
            {
                while (isForegroundServiceRunning)
                {
                    System.Diagnostics.Debug.WriteLine("Foreground service is Running");
                    Thread.Sleep(2000);
                }
            });
            string channelID = "ForegroundServiceChannel";
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            
            if (OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                var notificationChannel = new NotificationChannel(channelID, channelID, NotificationImportance.Low);
                notificationManager.CreateNotificationChannel(notificationChannel);
            }

            var notificationBuilder = new NotificationCompat.Builder(this, channelID).SetContentTitle("ForegroundServiceStarted").SetSmallIcon(Resource.Mipmap.appicon).SetContentText("Service Running in Foreground").SetPriority(1).SetOngoing(true).SetChannelId(channelID).SetAutoCancel(true);
            StartForeground(10001, notificationBuilder.Build());
            return base.OnStartCommand(intent, flags, startId);
        }

        public override void OnCreate()
        {
            isForegroundServiceRunning = true;
            base.OnCreate();
        }
        public override void OnDestroy()
        {
            isForegroundServiceRunning = false;
            base.OnDestroy();
        }
        public void Start()
        {
            var intent = new Intent(AndroidApp.Context, typeof(ForegroundService));
            AndroidApp.Context.StartForegroundService(intent);
        }

        public void Stop()
        {
            var intent = new Intent(AndroidApp.Context, typeof(ForegroundService));
            AndroidApp.Context.StopService(intent);
        }

        public bool IsForegroundServiceRunning()
        {
            return isForegroundServiceRunning;
        }
    }
}
