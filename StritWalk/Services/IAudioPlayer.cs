using System;
using System.Collections.Generic;
using System.Text;


namespace StritWalk
{
    public interface IAudioPlayer
    {
        event EventHandler FinishedPlaying;

        void Play();
        void Play(string pathToAudioFile);
        void Pause();
        void SolveErrors();


    }
}