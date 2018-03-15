using System;
using System.Collections.Generic;
using System.Text;
using AVFoundation;

namespace StritWalk
{
    public interface IAudioPlayer
    {
        event EventHandler FinishedPlaying;

        void Play();
        void Play(string pathToAudioFile);
        void Pause();


    }
}