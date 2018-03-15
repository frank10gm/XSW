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

[assembly: Dependency(typeof(StritWalk.Droid.AudioPlayer))]

namespace StritWalk.Droid
{
    public partial class AudioPlayer : IAudioPlayer
    {
        private MediaPlayer _mediaPlayer;

        public event EventHandler FinishedPlaying;

        public AudioPlayer()
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
    }
}