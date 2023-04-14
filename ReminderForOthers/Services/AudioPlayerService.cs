using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Maui.Audio;
using System.Diagnostics;
#if ANDROID
using Activity = Android.App.Activity;
#endif



namespace ReminderForOthers.Services
{
    public class AudioPlayerService
    {

        private IAudioManager audioManager;
        private IAudioPlayer audioPlayer;

        private IAudioPlayer[] players;

        private static string[] audioClips = { "notification.wav", "start-record-audio.wav", "stop-record-audio.wav" };

#if ANDROID
        private static Android.Media.AudioManager androidAudioManager = (Android.Media.AudioManager) MainActivity.ActivityCurrent.GetSystemService(Activity.AudioService);

#endif

        public AudioPlayerService()
        {
            audioManager = AudioManager.Current;
            players = new IAudioPlayer[3];
        }

        public bool PlayStartRecordAudio()
        {
            if (players[1] != null) { return false; }
            AudioStart(1);

            return true;
        }

        public bool PlayStopRecordAudio()
        {
            if (players[2] != null) { return false; }
            AudioStart(2);

            return true;
        }
        public bool PlayNotificationAudio()
        {
            if (players[0] != null) { return false; }
            AudioStart(0);
            return true;
        }

        //helper method that starts plays the audio and stops after time duration
        private void AudioStart(int index)
        {
            try
            {
                players[index] = audioManager.CreatePlayer(FileSystem.OpenAppPackageFileAsync(audioClips[index]).Result);
                players[index].Play();
                int duration = Convert.ToInt32(players[index].Duration);
                Thread.Sleep(1000 * duration);
                players[index].Stop();
                players[index].Dispose();
                players[index] = null;
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error: " + ex.Message);
            }

        }

        public int AudioDuration(string filePath)
        {
            try
            {
                Stream stream = File.OpenRead(filePath);
                IAudioPlayer tempPlayer = audioManager.CreatePlayer(stream);
                double duration = tempPlayer.Duration;
                return Convert.ToInt32(duration);
            }
            catch (Exception ex)
            {

                Console.WriteLine("AudioDuration Error: " + ex.Message);
            }
            return 0;
        }

        public void EnableBackgroundAudio()
        {

//#if ANDROID
//            //enable the background audio 
//            Android.Media.Session.MediaSessionManager manager;
//            manager
//            Android.Media.Session.PlaybackStateCode.Playing;
            
//#endif
        }
    }
}
