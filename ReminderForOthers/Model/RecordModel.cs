using Plugin.AudioRecorder;
using Plugin.SimpleAudioRecorder;
using ReminderForOthers.Services;

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
        private AudioPlayerService audioService;

        public RecordModel()
        {
            audioRecorder = CrossSimpleAudioRecorder.CreateSimpleAudioRecorder();
            player = new AudioPlayer();
            audioService = new AudioPlayerService();
            isRecording = false;
            hasRecordedAudio=false;
            player.FinishedPlaying += (s, e) =>
            {
                audioService.PlayStopRecordAudio();
            };
        }

        public async Task<bool> RecordAudioAsync()
        {
            if (isRecording) { return false; }
            isRecording = true;
            audioService.PlayStartRecordAudio();
            await audioRecorder.RecordAsync();
            return true;
        }

        public async Task<bool> StopRecordAudioAsync()
        {
            if (!isRecording) { return false; }
            isRecording = false;
            hasRecordedAudio = true;
            recordedAudio = await audioRecorder.StopAsync();
            audioService.PlayStopRecordAudio();
            return true;
        }

        public bool PlayAudio()
        {
            //doesnt play audio
            if (!hasRecordedAudio) { return false; }

            audioService.PlayStartRecordAudio();
            player.Play(recordedAudio.GetFilePath());

            return true; //playing
        }

        public bool PauseAudio()
        {
            //doesnt play audio
            if (!hasRecordedAudio) { return false; }
            player.Pause();
            audioService.PlayStopRecordAudio();
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
        public bool PlayDownloadedAudio(string filePath) 
        {
            if (!File.Exists(filePath)) { return false; }
            audioService.PlayStartRecordAudio();
            player.Play(filePath);
            
            //Thread.Sleep(audioService.AudioDuration(filePath));
           
            return true;
        }

        public bool GetisRecording()
        {
            return this.isRecording;
        }
    }
}
