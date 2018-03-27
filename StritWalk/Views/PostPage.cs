using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace StritWalk
{
    public class PostPage : ContentPage
    {
        //variabili e prop
        PostPageVM viewModel;
        ProgressBar progress;
        Label bytesLabel;
        SKCanvasView canvasView;
        Stream stream;
        byte[] wav;

        public PostPage(ObservableRangeCollection<Item> Items, Item Item)
        {
            //viewmodel
            BindingContext = viewModel = new PostPageVM();
            viewModel.Navigation = Navigation;

            //visual elements
            var mainLabel = new Label { FormattedText = Item.Post };
            progress = new ProgressBar { Progress = 0 };
            bytesLabel = new Label { };
            canvasView = new SKCanvasView();
            canvasView.HeightRequest = 200;
            canvasView.PaintSurface += OnCanvasViewPaintSurface;

            bool checkAudio = CheckAudio(Item.Audio);

            //relative layout
            var layout = new RelativeLayout();

            layout.Children.Add(mainLabel,
                Constraint.Constant(20),
                Constraint.Constant(20),
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width - 40;
                }),
                null);

            if (Item.AudioExist)
            {
                layout.Children.Add(progress,
                Constraint.Constant(20),
                Constraint.RelativeToView(mainLabel, (Parent, sibling) =>
                {
                    return sibling.Y + sibling.Height + 40;
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width - 40;
                }),
                Constraint.Constant(10));

                layout.Children.Add(bytesLabel,
                Constraint.Constant(20),
                Constraint.RelativeToView(progress, (Parent, sibling) =>
                {
                    return sibling.Y + sibling.Height + 20;
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width - 40;
                }),
                Constraint.Constant(20));

                layout.Children.Add(canvasView,
                Constraint.Constant(20),
                Constraint.RelativeToView(bytesLabel, (Parent, sibling) =>
                {
                    return sibling.Y + sibling.Height + 20;
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width - 40;
                }),
                Constraint.Constant(200));
            }

            layout.BackgroundColor = (Color)Application.Current.Resources["Sfondo4"];

            Content = layout;
        }

        bool CheckAudio(string audio)
        {
            if (audio == "") return false;
            Console.WriteLine("@@@@ checking audio : " + audio);

            var client = new WebClient();

            client.DownloadProgressChanged += async (object sender, DownloadProgressChangedEventArgs e) =>
            {
                double val = Convert.ToDouble(e.ProgressPercentage) / 100;
                bytesLabel.TextColor = (Color)Application.Current.Resources["Testo3"];
                bytesLabel.Text = (e.BytesReceived / 1024f) / 1024f + " / " + (e.TotalBytesToReceive / 1024f) / 1024f + " MB.";
                await progress.ProgressTo(val, 250, Easing.Linear);
            };

            client.DownloadDataCompleted += (object sender, DownloadDataCompletedEventArgs e) =>
            {
                wav = e.Result;
                stream = new MemoryStream(wav);
                canvasView.InvalidateSurface();
            };

            client.DownloadDataAsync(new Uri("https://www.hackweb.it/api/uploads/audio/" + audio));

            return true;
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {

            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear();

            if (stream == null)
            {
                //SKPaint paint2 = new SKPaint
                //{
                //    Style = SKPaintStyle.Stroke,
                //    Color = Color.Red.ToSKColor(),
                //    StrokeWidth = 25
                //};

                //canvas.DrawCircle(info.Width / 2, info.Height / 2, 100, paint2);

                //paint2.Style = SKPaintStyle.Fill;
                //paint2.Color = SKColors.Blue;
                //canvas.DrawCircle(args.Info.Width / 2, args.Info.Height / 2, 100, paint2);
                return;
            }

            SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = Color.FromHex("#00ff00").ToSKColor(),
                StrokeWidth = 1
            };

            BinaryReader reader = new BinaryReader(stream);
            int chunkID = reader.ReadInt32();
            int fileSize = reader.ReadInt32();
            int riffType = reader.ReadInt32();
            int fmtID = reader.ReadInt32();
            int fmtSize = reader.ReadInt32();
            int fmtCode = reader.ReadInt16();
            int channels = reader.ReadInt16();
            int sampleRate = reader.ReadInt32();
            int fmtAvgBPS = reader.ReadInt32();
            int fmtBlockAlign = reader.ReadInt16();
            int bitDepth = reader.ReadInt16();

            if (fmtSize == 18)
            {
                // Read any extra values               
                int fmtExtraSize = reader.ReadInt16();
                reader.ReadBytes(fmtExtraSize);
            }

            //sound data
            int dataID = reader.ReadInt32();
            int dataSize = reader.ReadInt32();
            byte[] sound = reader.ReadBytes(dataSize);
            int numSamples = sound.Length / (channels * bitDepth / 8);

            Console.WriteLine("@@@@ bits: " + bitDepth + "; sound length: " + sound.Length + "; h: " + info.Height + ";");

            //visualize waveform
            var w = info.Width;
            var h = info.Height;
            var mid = h / 2;
            var batch = numSamples / w;
            var max = 32760;
            short[] buffer = new short[numSamples];

            for (int i2 = 0; i2 < numSamples; i2++)
            {
                buffer[i2] = BitConverter.ToInt16(sound, i2 * 2);
            }

            int i = 1;
            int j = mid + ((buffer[0] * mid) / max);

            //int j = h - Convert.ToInt32(sound[0]);

            canvas.DrawLine(0, mid, i, j, paint);
            SKPoint prev = new SKPoint(i, j);
            SKPoint next = new SKPoint();
            i++;

            for (int n = 0; n < numSamples; n += batch) // n < batch; n++;
            {
                Console.WriteLine("@@@@ batch: " + n);
                //foreach (byte temp in sound)            
                j = mid + ((buffer[n] * mid) / max);
                next.X = i++;
                next.Y = j;
                //gra.DrawLine(a, prev, next);
                canvas.DrawLine(prev, next, paint);
                prev = next;
            }

        }
    }
}

