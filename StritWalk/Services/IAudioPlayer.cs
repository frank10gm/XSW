using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StritWalk
{
    public interface IAudioPlayer
    {
        event EventHandler FinishedPlaying;
        event EventHandler FinishedRecording;

        void Play();
        void Play(string pathToAudioFile);
        void Pause();
        void SolveErrors();
        void InitRecord();
        string StartRecording(double seconds = 10);
        void StopRecording();
        byte[] AudioDecoder(byte[] source);
        Task<byte[]> AudioDecoder3(byte[] source);
    }
}