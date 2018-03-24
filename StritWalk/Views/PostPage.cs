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

        public PostPage(ObservableRangeCollection<Item> Items, Item Item)
        {
            //viewmodel
            BindingContext = viewModel = new PostPageVM();
            viewModel.Navigation = Navigation;


            //visual elements
            var mainLabel = new Label { FormattedText = Item.Post };
            progress = new ProgressBar { Progress = 0 };            

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

            layout.Children.Add(progress,
                Constraint.Constant(20),
                Constraint.RelativeToView(mainLabel, (Parent, sibling) =>
                {
                    return sibling.Y + sibling.Height + 10;
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width - 40;
                }),
                Constraint.Constant(10));

            Content = layout;
        }

        bool CheckAudio(string audio)
        {            
            if (audio == "") return false;
            Console.WriteLine("@@@@ checking audio : " + audio);

            var client = new WebClient();
            
            client.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) => {
                Console.WriteLine("@@@@ perc " + e.ProgressPercentage);
                progress.ProgressTo(e.ProgressPercentage / 100, 0, Easing.Linear);
            };
            client.DownloadFileCompleted += (sender, e) => Console.WriteLine("@@@@ file download Finished");  
            client.DownloadDataAsync(new Uri("https://www.hackweb.it/api/uploads/audio/" + audio), "audioFile.wav");

            return true;
        }
    }
}

