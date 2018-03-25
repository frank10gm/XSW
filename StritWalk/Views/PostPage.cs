using System;
using System.Net;
using Xamarin.Forms;

namespace StritWalk
{
    public class PostPage : ContentPage
    {
        //variabili e prop
        PostPageVM viewModel;
        ProgressBar progress;
        Label bytesLabel;

        public PostPage(ObservableRangeCollection<Item> Items, Item Item)
        {
            //viewmodel
            BindingContext = viewModel = new PostPageVM();
            viewModel.Navigation = Navigation;


            //visual elements
            var mainLabel = new Label { FormattedText = Item.Post };
            progress = new ProgressBar { Progress = 0 };
            bytesLabel = new Label { };


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
                Constraint.Constant(40),
                Constraint.RelativeToView(progress, (Parent, sibling) =>
                {
                    return sibling.Y + sibling.Height + 20;
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width - 40;
                }),
                Constraint.Constant(20));
            }


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
                Console.WriteLine("@@@@ perc " + val);
                bytesLabel.Text = (e.BytesReceived / 1024f) / 1024f + " / " + (e.TotalBytesToReceive / 1024f) / 1024f + " MB.";
                await progress.ProgressTo(val, 250, Easing.Linear);
            };
            client.DownloadDataCompleted += (object sender, DownloadDataCompletedEventArgs e) =>
            {
                Console.WriteLine("@@@@ file download Finished");
                byte[] raw = e.Result;
            };
            client.DownloadDataAsync(new Uri("https://www.hackweb.it/api/uploads/audio/" + audio));

            return true;
        }
    }
}

