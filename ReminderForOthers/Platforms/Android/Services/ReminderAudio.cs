using Android.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderForOthers.Platforms.Android.Services
{
    public class ReminderAudio
    {
        public static int AudioDuration(string filePath)
        {
            MediaPlayer mediaPlayer = new MediaPlayer();
            mediaPlayer.SetDataSource(filePath);
            mediaPlayer.Prepare();
            return mediaPlayer.Duration;
        }
    }
}
