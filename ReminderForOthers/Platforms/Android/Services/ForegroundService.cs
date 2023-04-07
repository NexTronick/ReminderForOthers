using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using ReminderForOthers.Platforms.Android.Services;
using ReminderForOthers.Services;
using AndroidApp = Android.App.Application;

//[assembly: Dependency(typeof(ForegroundService))]
namespace ReminderForOthers.Platforms.Android.Services
{
    [Service]
    public class ForegroundService : Service, IForegroundService
    {
        private static bool isForegroundServiceRunning;
        private static ReminderNotificationService reminderNotificationService = new ReminderNotificationService();
        private static bool isReminderNotificationServiceRunning;
        private static bool isFirstTimeRun;

        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            //Task.Run(() =>
            //{
            //    while (isForegroundServiceRunning)
            //    {
            //        System.Diagnostics.Debug.WriteLine("Foreground service is Running");
            //        Thread.Sleep(2000);
            //    }
            //});
            
            //reminder service 
            reminderNotificationService.RunReminderServices(1000*60,1000);
            reminderNotificationService.StartService();
            
            //friend request accepted service [to be added]


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
            isReminderNotificationServiceRunning = true;
            base.OnCreate();
        }
        public override void OnDestroy()
        {
            isForegroundServiceRunning = false;
            isReminderNotificationServiceRunning = false;
            reminderNotificationService.StopService();
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
