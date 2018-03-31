
using System;
using Foundation;
using AVFoundation;
using Xamarin.Forms;
using System.IO;
using System.Runtime.InteropServices;

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

        public byte[] AudioDecoder2(byte[] source)
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
            var engine = new AVAudioEngine();
            var mixer = new AVAudioMixerNode();
            var player = new AVAudioPlayerNode();
            var input = engine.InputNode;
            mixer.Volume = 0;
            engine.AttachNode(mixer);
            engine.AttachNode(player);
            engine.Connect(player, mixer, formatIn);
            engine.Connect(input, mixer, input.GetBusOutputFormat(0));
            engine.Connect(mixer, engine.MainMixerNode, formatIn);
            var converter = new AVAudioConverter(formatIn, formatOut);
            var sampleRateConversionRatio = 1;
            byte[] finalData = new byte[32000];
            mixer.InstallTapOnBus(0, 32000, formatIn, new AVAudioNodeTapBlock((AVAudioPcmBuffer buffer, AVAudioTime when) =>
            {
                var capacity = new UInt32();
                capacity = (uint)(((float)buffer.FrameCapacity) / sampleRateConversionRatio);

                AVAudioConverterInputHandler inputHandler;
                inputHandler = (uint inNumberOfPackets, out AVAudioConverterInputStatus outStatus) =>
                {
                    outStatus = AVAudioConverterInputStatus.HaveData;
                    return buffer;
                };
                var error = new NSError();
                var status = converter.ConvertToBuffer(buffer, out error, inputHandler);
                var myData = buffer.Int16ChannelData;
                //Marshal.Copy(buffer, finalData, 0, 32000);
            }));
            return finalData;
        }

        public byte[] AudioDecoder(byte[] source)
        {
            //throw new NotImplementedException();
            var filePath = Path.Combine(Path.GetTempPath(), "toConvert.m4a");
            File.WriteAllBytes(filePath, source);
            var fileUrl = new NSUrl(filePath);
            var err = new NSError();
            var file = new AVAudioFile(fileUrl, out err);
            var pcmBuffer = new AVAudioPcmBuffer(new AVAudioFormat(AVAudioCommonFormat.PCMInt16, 16000, 1, false), (uint)source.Length);
            file.ReadIntoBuffer(pcmBuffer, out err);
            file.WriteFromBuffer(pcmBuffer, out err);
            byte[] data = File.ReadAllBytes(filePath);
            return source;
        }
    }
}
