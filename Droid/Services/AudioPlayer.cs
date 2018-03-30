using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Media;

using StritWalk;

using Xamarin.Forms;
using System.IO;

[assembly: Dependency(typeof(StritWalk.Droid.AudioPlayer))]

namespace StritWalk.Droid
{
    public partial class AudioPlayer : IAudioPlayer
    {
        private MediaPlayer _mediaPlayer;

        public event EventHandler FinishedPlaying;
        public event EventHandler FinishedRecording;
        string _audioFilePath;
        MediaRecorder _recorder;

        public AudioPlayer()
        {
        }

        public void SolveErrors()
        {
            //AVAudioSession.SharedInstance().SetCategory(AVAudioSessionCategory.PlayAndRecord);
        }


        public void Play(string pathToAudioFile)
        {
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Completion -= MediaPlayer_Completion;
                _mediaPlayer.Stop();
            }

            if (pathToAudioFile != null)
            {
                if (_mediaPlayer == null)
                {
                    _mediaPlayer = new MediaPlayer();

                    _mediaPlayer.Prepared += (sender, args) =>
                    {
                        _mediaPlayer.Start();
                        _mediaPlayer.Completion += MediaPlayer_Completion;
                    };
                }

                _mediaPlayer.Reset();
                //_mediaPlayer.SetVolume (1.0f, 1.0f);

                _mediaPlayer.SetDataSource(pathToAudioFile);
                _mediaPlayer.PrepareAsync();
            }
        }


        void MediaPlayer_Completion(object sender, EventArgs e)
        {
            FinishedPlaying?.Invoke(this, EventArgs.Empty);
        }

        public void Pause()
        {
            _mediaPlayer?.Pause();
        }

        public void Play()
        {
            _mediaPlayer?.Start();
        }

        public void InitRecord()
        {
            string fileName = string.Format("myfile{0}.m4a", DateTime.Now.ToString("yyyyMMddHHmmss"));
            _audioFilePath = Path.Combine(Path.GetTempPath(), fileName);
            _recorder = new MediaRecorder();
            _recorder.SetAudioChannels(1);
            _recorder.SetAudioSource(AudioSource.Mic);
            _recorder.SetOutputFormat(OutputFormat.Mpeg4);
            _recorder.SetAudioEncoder(AudioEncoder.Aac);
            _recorder.SetOutputFile(_audioFilePath);
            _recorder.SetAudioSamplingRate(44000);
            _recorder.Prepare();
            _recorder.Info += Recorder_FinishedRecording;
        }

        public string StartRecording(double seconds = 10)
        {
            _recorder.Start();
            return _audioFilePath;
        }

        public void StopRecording()
        {
            _recorder.Stop();
            _recorder.Reset();
        }

        private void Recorder_FinishedRecording(object sender, MediaRecorder.InfoEventArgs e)
        {
            FinishedRecording?.Invoke(this, EventArgs.Empty);
        }
    }
}