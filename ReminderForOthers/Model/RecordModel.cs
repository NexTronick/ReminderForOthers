
using Plugin.AudioRecorder;
using Plugin.SimpleAudioRecorder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderForOthers.Model
{
    public class RecordModel
    {
        //audio recording
        private ISimpleAudioRecorder audioRecorder;
        private AudioRecording recordedAudio;
        
        //auido playing
        private AudioPlayer player;
        public RecordModel()
        {
            audioRecorder = CrossSimpleAudioRecorder.CreateSimpleAudioRecorder();
            player = new AudioPlayer();
        }

        public async Task RecordAudioAsync() 
        {
            await audioRecorder.RecordAsync();
        }

        public async Task StopRecordAudioAsync() 
        {
            recordedAudio = await audioRecorder.StopAsync();
        }

        public bool PlayAudio() 
        {
            //doesnt play audio
            if (recordedAudio == null) { return false; }

            player.Play(recordedAudio.GetFilePath());

            return true; //playing
        }

        public bool PauseAudio() 
        {
            //doesnt play audio
            if (recordedAudio == null) { return false; }
            
            player.Pause();
            return true;
        }

        //check if it has recorded audio
        public bool HasRecordedAudio() 
        {
            return recordedAudio.HasRecording;
        }
        //Record file path
        public string GetRecordPath() 
        {
            //if there is no recording
            if (recordedAudio == null) { return null; }

            return recordedAudio.GetFilePath();
        }

        public async Task DisposeAudio() 
        {
            if (audioRecorder.IsRecording) { await StopRecordAudioAsync(); }
            recordedAudio.Dispose();
            audioRecorder = CrossSimpleAudioRecorder.CreateSimpleAudioRecorder();
            recordedAudio = null;
        }
    }
}
