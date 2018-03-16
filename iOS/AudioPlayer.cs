﻿
using System;
using Foundation;
using AVFoundation;
using Xamarin.Forms;

[assembly: Dependency(typeof(StritWalk.iOS.AudioPlayer))]

namespace StritWalk.iOS
{
    public partial class AudioPlayer : IAudioPlayer
    {
        private AVAudioPlayer _audioPlayer = null;

        public event EventHandler FinishedPlaying;

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
    }
}
