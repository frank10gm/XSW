
using System;
using Foundation;
using AVFoundation;
using Xamarin.Forms;
using System.IO;
using System.Runtime.InteropServices;
using CoreMedia;
using AudioToolbox;
using static CoreFoundation.DispatchSource;
using CoreFoundation;
using UIKit;
using System.Diagnostics;

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

        AudioFormatType outputFormat;
        double sampleRate;
        CFUrl sourceURL;
        NSUrl destinationURL;
        string destinationFilePath;
        AudioFileIO afio;

        class AudioFileIO
        {
            public AudioFileIO(int bufferSize)
            {
                this.SrcBufferSize = bufferSize;
                this.SrcBuffer = Marshal.AllocHGlobal(bufferSize);
            }

            ~AudioFileIO()
            {
                Marshal.FreeHGlobal(SrcBuffer);
            }

            public AudioFile SourceFile { get; set; }

            public int SrcBufferSize { get; private set; }

            public IntPtr SrcBuffer { get; private set; }

            public int SrcFilePos { get; set; }

            public AudioStreamBasicDescription SrcFormat { get; set; }

            public int SrcSizePerPacket { get; set; }

            public int NumPacketsPerRead { get; set; }

            public AudioStreamPacketDescription[] PacketDescriptions { get; set; }
        }

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
            var finalData2 = new byte[32000];
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
                //var myData = buffer.Int16ChannelData;
                //Marshal.Copy(buffer, finalData2, 0, 32000);
                File.WriteAllText(filePath, "");
                file.WriteFromBuffer(buffer, out error);
            }));

            byte[] finalData = File.ReadAllBytes(filePath);
            return finalData;
        }

        //audio magic cookie
        static void ReadCookie(AudioFile sourceFile, AudioConverter converter)
        {
            // grab the cookie from the source file and set it on the converter
            var cookie = sourceFile.MagicCookie;

            // if there is an error here, then the format doesn't have a cookie - this is perfectly fine as some formats do not
            if (cookie != null && cookie.Length != 0)
            {
                converter.DecompressionMagicCookie = cookie;
            }
        }

        // Input data proc callback
        AudioConverterError EncoderDataProc(ref int numberDataPackets, AudioBuffers data, ref AudioStreamPacketDescription[] dataPacketDescription)
        {
            // figure out how much to read
            int maxPackets = afio.SrcBufferSize / afio.SrcSizePerPacket;
            if (numberDataPackets > maxPackets)
                numberDataPackets = maxPackets;

            // read from the file
            int outNumBytes = 16384;

            // modified for iOS7 (ReadPackets depricated)
            afio.PacketDescriptions = afio.SourceFile.ReadPacketData(false, afio.SrcFilePos, ref numberDataPackets, afio.SrcBuffer, ref outNumBytes);

            if (afio.PacketDescriptions.Length == 0 && numberDataPackets > 0)
                throw new ApplicationException(afio.PacketDescriptions.ToString());

            // advance input file packet position
            afio.SrcFilePos += numberDataPackets;

            // put the data pointer into the buffer list
            data.SetData(0, afio.SrcBuffer, outNumBytes);

            // don't forget the packet descriptions if required
            if (dataPacketDescription != null)
                dataPacketDescription = afio.PacketDescriptions;

            return AudioConverterError.None;
        }

        // Some audio formats have a magic cookie associated with them which is required to decompress audio data
        // When converting audio, a magic cookie may be returned by the Audio Converter so that it may be stored along with
        // the output data -- This is done so that it may then be passed back to the Audio Converter at a later time as required
        static void WriteCookie(AudioConverter converter, AudioFile destinationFile)
        {
            var cookie = converter.CompressionMagicCookie;
            if (cookie != null && cookie.Length != 0)
            {
                destinationFile.MagicCookie = cookie;
            }
        }

        // Write output channel layout to destination file
        static void WriteDestinationChannelLayout(AudioConverter converter, AudioFile sourceFile, AudioFile destinationFile)
        {
            // if the Audio Converter doesn't have a layout see if the input file does
            var layout = converter.OutputChannelLayout ?? sourceFile.ChannelLayout;

            if (layout != null)
            {
                // TODO:
                throw new NotImplementedException();
                //destinationFile.ChannelLayout = layout;
            }
        }

        // Sets the packet table containing information about the number of valid frames in a file and where they begin and end
        // for the file types that support this information.
        // Calling this function makes sure we write out the priming and remainder details to the destination file
        static void WritePacketTableInfo(AudioConverter converter, AudioFile destinationFile)
        {
            if (!destinationFile.IsPropertyWritable(AudioFileProperty.PacketTableInfo))
                return;

            // retrieve the leadingFrames and trailingFrames information from the converter,
            AudioConverterPrimeInfo primeInfo = converter.PrimeInfo;

            // we have some priming information to write out to the destination file
            // The total number of packets in the file times the frames per packet (or counting each packet's
            // frames individually for a variable frames per packet format) minus mPrimingFrames, minus
            // mRemainderFrames, should equal mNumberValidFrames.

            AudioFilePacketTableInfo? pti_n = destinationFile.PacketTableInfo;
            if (pti_n == null)
                return;

            AudioFilePacketTableInfo pti = pti_n.Value;

            // there's priming to write out to the file
            // get the total number of frames from the output file
            long totalFrames = pti.ValidFrames + pti.PrimingFrames + pti.RemainderFrames;
            Debug.WriteLine("Total number of frames from output file: {0}", totalFrames);

            pti.PrimingFrames = primeInfo.LeadingFrames;
            pti.RemainderFrames = primeInfo.TrailingFrames;
            pti.ValidFrames = totalFrames - pti.PrimingFrames - pti.RemainderFrames;

            destinationFile.PacketTableInfo = pti;
        }

        public byte[] AudioDecoder(byte[] source)
        {
            ////throw new NotImplementedException();
            var filePath = Path.Combine(Path.GetTempPath(), "toConvert.m4a");
            File.WriteAllBytes(filePath, source);
            //var fileUrl = new NSUrl(filePath);
            //var err = new NSError();
            //var file = new AVAudioFile(fileUrl, out err);
            //var format = file.ProcessingFormat;
            ////var pcmBuffer = new AVAudioPcmBuffer(new AVAudioFormat(AVAudioCommonFormat.PCMInt16, 16000, 1, false), (uint)file.Length);
            //var pcmBuffer = new AVAudioPcmBuffer(format, (uint)file.Length);
            //file.ReadIntoBuffer(pcmBuffer, out err);
            //file.WriteFromBuffer(pcmBuffer, out err);
            //byte[] data = File.ReadAllBytes(filePath);
            //return data;

            sourceURL = CFUrl.FromFile(filePath);
            var paths = NSSearchPath.GetDirectories(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User);
            destinationFilePath = paths[0] + "/output.wav";
            destinationURL = NSUrl.FromFilename(destinationFilePath);

            //info about audio file
            var fileID = AudioFile.Open(sourceURL, AudioFilePermission.Read);
            string fileName = sourceURL.FileSystemPath;
            var asbd = fileID.DataFormat.Value;
            var info = string.Format("{0} {1} {2} Hz ({3} ch.)",
                "", asbd.Format, asbd.SampleRate, asbd.ChannelsPerFrame);
            Console.WriteLine("@@@@@ file info : " + info);

            //datas
            outputFormat = AudioFormatType.LinearPCM;
            sampleRate = 44100.0;

            //start conversion
            AudioSession.Category = AudioSessionCategory.AudioProcessing;
            AudioFile sourceFile = AudioFile.Open(sourceURL, AudioFilePermission.Read);
            var srcFormat = (AudioStreamBasicDescription)sourceFile.DataFormat;
            var dstFormat = new AudioStreamBasicDescription();
            dstFormat.SampleRate = sampleRate;

            if (outputFormat == AudioFormatType.LinearPCM)
            {
                // if the output format is PCM create a 16-bit int PCM file format description as an example
                dstFormat.Format = AudioFormatType.LinearPCM;
                dstFormat.ChannelsPerFrame = srcFormat.ChannelsPerFrame;
                dstFormat.BitsPerChannel = 16;
                dstFormat.BytesPerPacket = dstFormat.BytesPerFrame = 2 * dstFormat.ChannelsPerFrame;
                dstFormat.FramesPerPacket = 1;
                dstFormat.FormatFlags = AudioFormatFlags.LinearPCMIsPacked | AudioFormatFlags.LinearPCMIsSignedInteger;
            }
            else
            {
                // compressed format - need to set at least format, sample rate and channel fields for kAudioFormatProperty_FormatInfo
                dstFormat.Format = outputFormat;
                dstFormat.ChannelsPerFrame = (outputFormat == AudioFormatType.iLBC ? 1 : srcFormat.ChannelsPerFrame); // for iLBC num channels must be 1

                // use AudioFormat API to fill out the rest of the description
                AudioFormatError afe = AudioStreamBasicDescription.GetFormatInfo(ref dstFormat);
                if (afe != AudioFormatError.None)
                {
                    Console.WriteLine("Cannot create destination format {0:x}", afe);
                }
            }

            AudioConverterError ce;
            var converter = AudioConverter.Create(srcFormat, dstFormat, out ce);
            Debug.Assert(ce == AudioConverterError.None);
            converter.InputData += EncoderDataProc;

            // if the source has a cookie, get it and set it on the Audio Converter
            ReadCookie(sourceFile, converter);

            // get the actual formats back from the Audio Converter
            srcFormat = converter.CurrentInputStreamDescription;
            dstFormat = converter.CurrentOutputStreamDescription;

            if (dstFormat.Format == AudioFormatType.MPEG4AAC)
            {
                uint outputBitRate = 192000; // 192k

                // ignore errors as setting may be invalid depending on format specifics such as samplerate
                try
                {
                    converter.EncodeBitRate = outputBitRate;
                }
                catch
                {
                }

                // get it back and print it out
                outputBitRate = converter.EncodeBitRate;
                Debug.Print("AAC Encode Bitrate: {0}", outputBitRate);
            }

            // can the Audio Converter resume conversion after an interruption?
            // this property may be queried at any time after construction of the Audio Converter after setting its output format
            // there's no clear reason to prefer construction time, interruption time, or potential resumption time but we prefer
            // construction time since it means less code to execute during or after interruption time
            bool canResumeFromInterruption;
            try
            {
                canResumeFromInterruption = converter.CanResumeFromInterruption;
                Debug.Print("Audio Converter {0} continue after interruption!", canResumeFromInterruption ? "CAN" : "CANNOT");
            }
            catch (Exception e)
            {
                // if the property is unimplemented (kAudioConverterErr_PropertyNotSupported, or paramErr returned in the case of PCM),
                // then the codec being used is not a hardware codec so we're not concerned about codec state
                // we are always going to be able to resume conversion after an interruption

                canResumeFromInterruption = false;
                Debug.Print("CanResumeFromInterruption: {0}", e.Message);
            }

            // create the destination file
            var destinationFile = AudioFile.Create(destinationURL, AudioFileType.WAVE, dstFormat, AudioFileFlags.EraseFlags);

            // set up source buffers and data proc info struct
            afio = new AudioFileIO(32 * 1024); // 32Kb
            afio.SourceFile = sourceFile;
            afio.SrcFormat = srcFormat;

            if (srcFormat.BytesPerPacket == 0)
            {
                // if the source format is VBR, we need to get the maximum packet size
                // use kAudioFilePropertyPacketSizeUpperBound which returns the theoretical maximum packet size
                // in the file (without actually scanning the whole file to find the largest packet,
                // as may happen with kAudioFilePropertyMaximumPacketSize)
                afio.SrcSizePerPacket = sourceFile.PacketSizeUpperBound;

                // how many packets can we read for our buffer size?
                afio.NumPacketsPerRead = afio.SrcBufferSize / afio.SrcSizePerPacket;

                // allocate memory for the PacketDescription structures describing the layout of each packet
                afio.PacketDescriptions = new AudioStreamPacketDescription[afio.NumPacketsPerRead];
            }
            else
            {
                // CBR source format
                afio.SrcSizePerPacket = srcFormat.BytesPerPacket;
                afio.NumPacketsPerRead = afio.SrcBufferSize / afio.SrcSizePerPacket;
            }

            // set up output buffers
            int outputSizePerPacket = dstFormat.BytesPerPacket; // this will be non-zero if the format is CBR
            const int theOutputBufSize = 32 * 1024; // 32Kb
            var outputBuffer = Marshal.AllocHGlobal(theOutputBufSize);
            AudioStreamPacketDescription[] outputPacketDescriptions = null;

            if (outputSizePerPacket == 0)
            {
                // if the destination format is VBR, we need to get max size per packet from the converter
                outputSizePerPacket = (int)converter.MaximumOutputPacketSize;

                // allocate memory for the PacketDescription structures describing the layout of each packet
                outputPacketDescriptions = new AudioStreamPacketDescription[theOutputBufSize / outputSizePerPacket];
            }
            int numOutputPackets = theOutputBufSize / outputSizePerPacket;

            // if the destination format has a cookie, get it and set it on the output file
            WriteCookie(converter, destinationFile);

            // write destination channel layout
            if (srcFormat.ChannelsPerFrame > 2)
            {
                WriteDestinationChannelLayout(converter, sourceFile, destinationFile);
            }

            long totalOutputFrames = 0; // used for debugging
            long outputFilePos = 0;
            AudioBuffers fillBufList = new AudioBuffers(1);
            bool error = false;

            // loop to convert data
            Debug.WriteLine("Converting...");
            while (true)
            {
                // set up output buffer list
                fillBufList[0] = new AudioBuffer()
                {
                    NumberChannels = dstFormat.ChannelsPerFrame,
                    DataByteSize = theOutputBufSize,
                    Data = outputBuffer
                };

                //// this will block if we're interrupted
                //var wasInterrupted = AppDelegate.ThreadStatePausedCheck();

                //if (wasInterrupted && !canResumeFromInterruption)
                //{
                //    // this is our interruption termination condition
                //    // an interruption has occured but the Audio Converter cannot continue
                //    Debug.WriteLine("Cannot resume from interruption");
                //    error = true;
                //    break;
                //}

                // convert data
                int ioOutputDataPackets = numOutputPackets;
                var fe = converter.FillComplexBuffer(ref ioOutputDataPackets, fillBufList, outputPacketDescriptions);
                // if interrupted in the process of the conversion call, we must handle the error appropriately
                if (fe != AudioConverterError.None)
                {
                    Debug.Print("FillComplexBuffer: {0}", fe);
                    error = true;
                    break;
                }

                if (ioOutputDataPackets == 0)
                {
                    // this is the EOF conditon
                    break;
                }

                // write to output file
                var inNumBytes = fillBufList[0].DataByteSize;

                var we = destinationFile.WritePackets(false, inNumBytes, outputPacketDescriptions, outputFilePos, ref ioOutputDataPackets, outputBuffer);
                if (we != 0)
                {
                    Debug.Print("WritePackets: {0}", we);
                    error = true;
                    break;
                }

                // advance output file packet position
                outputFilePos += ioOutputDataPackets;

                if (dstFormat.FramesPerPacket != 0)
                {
                    // the format has constant frames per packet
                    totalOutputFrames += (ioOutputDataPackets * dstFormat.FramesPerPacket);
                }
                else
                {
                    // variable frames per packet require doing this for each packet (adding up the number of sample frames of data in each packet)
                    for (var i = 0; i < ioOutputDataPackets; ++i)
                        totalOutputFrames += outputPacketDescriptions[i].VariableFramesInPacket;
                }

            }

            Marshal.FreeHGlobal(outputBuffer);

            if (!error)
            {
                // write out any of the leading and trailing frames for compressed formats only
                if (dstFormat.BitsPerChannel == 0)
                {
                    // our output frame count should jive with
                    Debug.Print("Total number of output frames counted: {0}", totalOutputFrames);
                    WritePacketTableInfo(converter, destinationFile);
                }

                // write the cookie again - sometimes codecs will update cookies at the end of a conversion
                WriteCookie(converter, destinationFile);
            }

            converter.Dispose();
            destinationFile.Dispose();
            sourceFile.Dispose();

            //// transition thread state to State.Done before continuing
            //AppDelegate.ThreadStateSetDone();
            //return !error;

            //info about audio file
            fileID = AudioFile.Open(destinationURL, AudioFilePermission.Read);
            fileName = System.IO.Path.GetFileName(destinationURL.Path);
            asbd = fileID.DataFormat.Value;
            info = string.Format("{0} {1} {2} Hz ({3} ch.)",
                "", asbd.Format, asbd.SampleRate, asbd.ChannelsPerFrame);
            Console.WriteLine("@@@@@ file info : " + info);

            byte[] data = File.ReadAllBytes(destinationFilePath);

            return data;
        }




    }
}
