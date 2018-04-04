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
using System.Threading.Tasks;
using System.Threading;
using Java.Nio;

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
        int bufferSize;
        AudioRecord audioRecord;
        bool endRecording = false;
        byte[] audioBuffer = null;
        public Action<bool> RecordingStateChanged;

        // Member variables representing frame data        
        private String mFileType;
        private int mFileSize;
        private int mAvgBitRate;  // Average bit rate in kbps.
        private int mSampleRate;
        private int mChannels;
        private int mNumSamples;  // total number of samples per channel in audio file
        private ByteBuffer mDecodedBytes;  // Raw audio data
        private ShortBuffer mDecodedSamples;
        // shared buffer with mDecodedBytes.
        // mDecodedSamples has the following format:
        // {s1c1, s1c2, ..., s1cM, s2c1, ..., s2cM, ..., sNc1, ..., sNcM}
        // where sicj is the ith sample of the jth channel (a sample is a signed short)
        // M is the number of channels (e.g. 2 for stereo) and N is the number of samples per channel.


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

        public string StartRecording(double seconds = 10)
        {
            if (_recorder == null)
            {
                string fileName = string.Format("myfile{0}.m4a", DateTime.Now.ToString("yyyyMMddHHmmss"));
                _audioFilePath = Path.Combine(Path.GetTempPath(), fileName);
                _recorder = new MediaRecorder();
                _recorder.SetAudioChannels(1);
                _recorder.SetAudioSource(AudioSource.Mic);
                _recorder.SetOutputFormat(OutputFormat.Mpeg4);
                _recorder.SetAudioEncoder(AudioEncoder.HeAac);
                _recorder.SetAudioEncodingBitRate(48000);
                _recorder.SetAudioSamplingRate(44100);
                _recorder.SetOutputFile(_audioFilePath);
                _recorder.Info += Recorder_FinishedRecording;
            }

            _recorder.Prepare();
            _recorder.Start();
            //RecordTimeout(seconds);
            _isRecording = true;
            _recordTask = Task.Run(() => RecordTimeout(seconds));
            return _audioFilePath;
        }

        public void StopRecording()
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

            bufferSize = AudioRecord.GetMinBufferSize(44100, ChannelIn.Mono, Android.Media.Encoding.Pcm16bit);
            audioBuffer = new Byte[bufferSize];
            audioRecord = new AudioRecord(
                // Hardware source of recording.
                AudioSource.Mic,
                // Frequency
                44100,
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

        public void StopRecording2()
        {
            endRecording = true;
            FinishedRecording?.Invoke(this, EventArgs.Empty);
            Thread.Sleep(500); // Give it time to drop out.
        }

        public string StartRecording2(double seconds = 10)
        {
            string fileName = string.Format("myfile{0}.mp4", DateTime.Now.ToString("yyyyMMddHHmmss"));
            _audioFilePath = Path.Combine(Path.GetTempPath(), fileName);
            Task task = Task.Run(async () => { await StartRecorderAsync(); });
            return _audioFilePath;
        }

        protected void StartDecodeAsync(byte[] source)
        {            

            //input and output files
            string inputFileName = string.Format("toConvert{0}.m4a", "");
            var inputFilePath = Path.Combine(Path.GetTempPath(), inputFileName);
            File.WriteAllBytes(inputFilePath, source);

            string outputFileName = string.Format("converted{0}.wav", "");
            var outputFilePath = Path.Combine(Path.GetTempPath(), outputFileName);
            File.WriteAllBytes(outputFilePath, source);

            //getting data from input file
            MediaFormat format = null;
            MediaExtractor extractor = new MediaExtractor();
            extractor.SetDataSource(inputFilePath);
            format = extractor.GetTrackFormat(0);
            int inputFileSize = (int)new FileInfo(inputFilePath).Length;
            var inputFileChannels = format.GetInteger(MediaFormat.KeyChannelCount);
            var inputFileSampleRate = format.GetInteger(MediaFormat.KeySampleRate);
            int expectedNumSamples = (int)((format.GetLong(MediaFormat.KeyDuration) / 1000000.0f) * inputFileSampleRate + 0.5f);

            //setting the codec
            MediaCodec codec = MediaCodec.CreateDecoderByType(format.GetString(MediaFormat.KeyMime));
            codec.Configure(format, null, null, 0);
            codec.Start();

            int decodedSampleSize = 0;
            byte[] decodedSamples = null;
            ByteBuffer[] inputBuffers = codec.GetInputBuffers();
            ByteBuffer[] outputBuffers = codec.GetOutputBuffers();
            int sample_size;
            MediaCodec.BufferInfo info = new MediaCodec.BufferInfo();
            long presentation_time;
            int tot_size_read = 0;
            bool done_reading = false;

            // Set the size of the decoded samples buffer to 1MB (~6sec of a stereo stream at 44.1kHz).
            // For longer streams, the buffer size will be increased later on, calculating a rough
            // estimate of the total size needed to store all the samples in order to resize the buffer
            // only once.
            mDecodedBytes = ByteBuffer.Allocate(1048576);
            Boolean firstSampleData = true;

            //conversion
            while (true)
            {
                // read data from file and feed it to the decoder input buffers.
                int inputBufferIndex = codec.DequeueInputBuffer(100);
                if (!done_reading && inputBufferIndex >= 0)
                {
                    sample_size = extractor.ReadSampleData(inputBuffers[inputBufferIndex], 0);
                    if (firstSampleData && format.GetString(MediaFormat.KeyMime).Equals("audio/mp4a-latm") && sample_size == 2)
                    {
                        // For some reasons on some devices (e.g. the Samsung S3) you should not
                        // provide the first two bytes of an AAC stream, otherwise the MediaCodec will
                        // crash. These two bytes do not contain music data but basic info on the
                        // stream (e.g. channel configuration and sampling frequency), and skipping them
                        // seems OK with other devices (MediaCodec has already been configured and
                        // already knows these parameters).
                        extractor.Advance();
                        tot_size_read += sample_size;
                    }
                    else if (sample_size < 0)
                    {
                        //all samples have been read
                        codec.QueueInputBuffer(inputBufferIndex, 0, 0, -1, MediaCodecBufferFlags.EndOfStream);
                        done_reading = true;
                    }
                    else
                    {
                        presentation_time = extractor.SampleTime;
                        codec.QueueInputBuffer(inputBufferIndex, 0, sample_size, presentation_time, 0);
                        extractor.Advance();
                        tot_size_read += sample_size;
                        
                    }
                }
            }


            Console.WriteLine("@@@ Format: " + inputFileChannels);
        }

        public byte[] AudioDecoder(byte[] source)
        {
            Task task = Task.Run(() => { StartDecodeAsync(source); });
            return source;
        }

    }
}