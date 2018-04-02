﻿using System;
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
using System.Threading.Tasks;
using System.Threading;

[assembly: Dependency(typeof(StritWalk.Droid.AudioPlayer))]

namespace StritWalk.Droid
{
    public partial class AudioPlayer : IAudioPlayer
    {
        private MediaPlayer _mediaPlayer;
        Task _recordTask;
        public event EventHandler FinishedPlaying;
        public event EventHandler FinishedRecording;
        string _audioFilePath;
        bool _isRecording;
        MediaRecorder _recorder;
        //low level
        readonly int bufferSize;
        ChannelIn channels = ChannelIn.Mono;
        Android.Media.Encoding audioFormat = Android.Media.Encoding.Ac3;
        MediaCodec mediaCodec;
        AudioRecord audioRecord;
        MediaMuxer mediaMuxer;
        bool endRecording = false;
        byte[] audioBuffer = null;
        public Action<bool> RecordingStateChanged;

        public AudioPlayer()
        {
        }

        public void SolveErrors()
        {

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
            //                     
        }

        public string StartRecording2(double seconds = 10)
        {
            if (_recorder == null)
            {
                string fileName = string.Format("myfile{0}.m4a", DateTime.Now.ToString("yyyyMMddHHmmss"));
                _audioFilePath = Path.Combine(Path.GetTempPath(), fileName);
                _recorder = new MediaRecorder();
                _recorder.SetAudioChannels(1);
                _recorder.SetAudioSource(AudioSource.Mic);
                _recorder.SetOutputFormat(OutputFormat.ThreeGpp);
                _recorder.SetAudioEncoder(AudioEncoder.AmrNb);
                _recorder.SetOutputFile(_audioFilePath);
                _recorder.SetAudioSamplingRate(44000);
                _recorder.Info += Recorder_FinishedRecording;
            }

            _recorder.Prepare();
            _recorder.Start();
            //RecordTimeout(seconds);
            _isRecording = true;
            _recordTask = Task.Run(() => RecordTimeout(seconds));
            return _audioFilePath;
        }

        public void StopRecording2()
        {
            _isRecording = false;
            _recorder.Stop();
            _recorder.Reset();
            _recorder.Release();
            _recorder.Info -= Recorder_FinishedRecording;
            _recorder = null;
            FinishedRecording?.Invoke(this, EventArgs.Empty);
        }

        private void Recorder_FinishedRecording(object sender, MediaRecorder.InfoEventArgs e)
        {
            Console.WriteLine("@@@ changed status _recorder: " + e.ToString());
            FinishedRecording?.Invoke(this, EventArgs.Empty);
        }

        private async void RecordTimeout(double seconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            if (!_isRecording) return;
            _recorder.Stop();
            _recorder.Reset();
            _recorder.Release();
            _recorder.Info -= Recorder_FinishedRecording;
            _recorder = null;
            FinishedRecording?.Invoke(this, EventArgs.Empty);
        }

        public byte[] AudioDecoder(byte[] source)
        {
            return source;
        }

        async Task ReadAudioAsync()
        {
            using (var fileStream = new FileStream(_audioFilePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
            {
                while (true)
                {
                    Console.WriteLine("@@@ is recording");
                    if (endRecording)
                    {
                        endRecording = false;
                        break;
                    }
                    try
                    {
                        // Keep reading the buffer while there is audio input.
                        int numBytes = await audioRecord.ReadAsync(audioBuffer, 0, audioBuffer.Length);
                        await fileStream.WriteAsync(audioBuffer, 0, numBytes);
                        // Do something with the audio input.
                    }
                    catch (Exception ex)
                    {
                        Console.Out.WriteLine("@@@ ex: " + ex.Message);
                        break;
                    }
                }
                fileStream.Close();
            }
            audioRecord.Stop();
            audioRecord.Release();
            _isRecording = false;
            RaiseRecordingStateChangedEvent();
        }

        private void RaiseRecordingStateChangedEvent()
        {
            if (RecordingStateChanged != null)
                RecordingStateChanged(_isRecording);
        }

        protected async Task StartRecorderAsync()
        {
            endRecording = false;
            _isRecording = true;

            RaiseRecordingStateChangedEvent();

            audioBuffer = new Byte[100000];
            audioRecord = new AudioRecord(
                // Hardware source of recording.
                AudioSource.Mic,
                // Frequency
                11025,
                // Mono or stereo
                ChannelIn.Mono,
                // Audio encoding
                Android.Media.Encoding.Pcm16bit,
                // Length of the audio clip.
                audioBuffer.Length
            );

            audioRecord.StartRecording();

            // Off line this so that we do not block the UI thread.
            await ReadAudioAsync();
        }

        public void StopRecording()
        {
            endRecording = true;
            FinishedRecording?.Invoke(this, EventArgs.Empty);
            Thread.Sleep(500); // Give it time to drop out.
        }

        public string StartRecording(double seconds = 10)
        {
            string fileName = string.Format("myfile{0}.mp4", DateTime.Now.ToString("yyyyMMddHHmmss"));
            _audioFilePath = Path.Combine(Path.GetTempPath(), fileName);
            Task task = Task.Run( async () => { await StartRecorderAsync(); });
            return _audioFilePath;
        }
    }
}