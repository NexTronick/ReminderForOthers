
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
        private bool isRecording;
        private bool hasRecordedAudio;

        //auido playing
        private AudioPlayer player;

        public RecordModel()
        {
            audioRecorder = CrossSimpleAudioRecorder.CreateSimpleAudioRecorder();
            player = new AudioPlayer();
            isRecording = false;
            hasRecordedAudio=false;
        }

        public async Task<bool> RecordAudioAsync()
        {
            if (isRecording) { return false; }
            isRecording = true;
            await audioRecorder.RecordAsync();
            return true;
        }

        public async Task<bool> StopRecordAudioAsync()
        {
            if (!isRecording) { return false; }
            isRecording = false;
            hasRecordedAudio = true;
            recordedAudio = await audioRecorder.StopAsync();
            return true;
        }

        public bool PlayAudio()
        {
            //doesnt play audio
            if (!hasRecordedAudio) { return false; }

            player.Play(recordedAudio.GetFilePath());

            return true; //playing
        }

        public bool PauseAudio()
        {
            //doesnt play audio
            if (!hasRecordedAudio) { return false; }

            player.Pause();
            return true;
        }

        //check if it has recorded audio
        public bool HasRecordedAudio()
        {
            if (recordedAudio == null) { return false; }
            return recordedAudio.HasRecording;
        }
        //Record file path
        public string GetRecordPath()
        {
            //if there is no recording
            if (recordedAudio == null) { return null; }

            return recordedAudio.GetFilePath();
        }

        public async Task DisposeAudioAsync()
        {
            if (audioRecorder == null)
            {
                audioRecorder = CrossSimpleAudioRecorder.CreateSimpleAudioRecorder(); 
                return;
            }
            if (isRecording) { 
                await StopRecordAudioAsync();
            }
            if (hasRecordedAudio) { 
                recordedAudio.Dispose();
                hasRecordedAudio = false;
            }
        }

        //to play downloaded audio
        public async Task PlayDownloadedAudio(string filePath) 
        {
            player.Play(filePath);
        }
    }
}
