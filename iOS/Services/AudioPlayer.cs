
using System;
using Foundation;
using AVFoundation;
using Xamarin.Forms;
using System.IO;

[assembly: Dependency(typeof(StritWalk.iOS.AudioPlayer))]

namespace StritWalk.iOS
{
    public partial class AudioPlayer : IAudioPlayer
    {
        private AVAudioPlayer _audioPlayer = null;
        public event EventHandler FinishedPlaying;
        AVAudioRecorder recorder;
        public event EventHandler FinishedRecording;
        string audioFilePath;

        public AudioPlayer()
        {
        }

        public void Play(string pathToAudioFile)
        {
            // Check if _audioPlayer is currently playing
            if (_audioPlayer != null)
            {
                _audioPlayer.FinishedPlaying -= Player_FinishedPlaying;
                _audioPlayer.Stop();
            }

            string localUrl = pathToAudioFile;

            AVAudioSession.SharedInstance().Init();
            NSError error;
            AVAudioSession.SharedInstance().OverrideOutputAudioPort(AVAudioSessionPortOverride.Speaker, out error);

            _audioPlayer = AVAudioPlayer.FromUrl(NSUrl.FromFilename(localUrl));
            _audioPlayer.FinishedPlaying += Player_FinishedPlaying;
            _audioPlayer.Play();
        }

        public void SolveErrors()
        {
            AVAudioSession.SharedInstance().SetCategory(AVAudioSessionCategory.PlayAndRecord);
        }


        private void Player_FinishedPlaying(object sender, AVStatusEventArgs e)
        {
            FinishedPlaying?.Invoke(this, EventArgs.Empty);
        }

        public void Pause()
        {
            _audioPlayer?.Pause();
        }

        public void Play()
        {
            _audioPlayer?.Play();
        }

        public void InitRecord()
        {
            NSError error;
            var audioSession = AVAudioSession.SharedInstance();
            var err = audioSession.SetCategory(AVAudioSessionCategory.PlayAndRecord);
            if (err != null)
            {
                return;
            }
            err = audioSession.SetActive(true);
            if (err != null)
            {
                return;
            }

            //recording
            string fileName = string.Format("myfile{0}.m4a", DateTime.Now.ToString("yyyyMMddHHmmss"));
            audioFilePath = Path.Combine(Path.GetTempPath(), fileName);

            var url = NSUrl.FromFilename(audioFilePath);
            var settings = new AudioSettings
            {
                SampleRate = 44100.0f,
                Format = AudioToolbox.AudioFormatType.MPEG4AAC,
                NumberChannels = 1,
                AudioQuality = AVAudioQuality.High
            };

            recorder = AVAudioRecorder.Create(url, settings, out error);
            recorder.PrepareToRecord();
            recorder.FinishedRecording += Recorder_FinishedRecording;
        }

        public string StartRecording(double time)
        {
            recorder.RecordFor(time);
            return audioFilePath;
        }

        public void StopRecording()
        {
            recorder.Stop();
        }

        private void Recorder_FinishedRecording(object sender, AVStatusEventArgs e)
        {
            FinishedRecording?.Invoke(this, EventArgs.Empty);
        }

        public void AudioDecoder(byte[] source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var err = new NSError();
            var formatOut = new AVAudioFormat(AVAudioCommonFormat.PCMInt16, 16000, 1, true);
            var filePath = Path.Combine(Path.GetTempPath(), "toConvert.m4a");
            File.WriteAllBytes(filePath, source);
            var fileUrl = new NSUrl(filePath);
            var file = new AVAudioFile(fileUrl, out err);
            var formatIn = file.ProcessingFormat;
            Console.WriteLine("@@@@ file format : " + formatIn);
            //var converter = new AVAudioConverter(new AVAudioFormat())
        }
    }
}
