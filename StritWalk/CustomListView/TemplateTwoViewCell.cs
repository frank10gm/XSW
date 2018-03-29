using System;
using Xamarin.Forms;
namespace StritWalk
{
    public class TemplateTwoViewCell : CustomViewCell
    {
        public TemplateTwoViewCell()
        {
            var w = Application.Current.MainPage.Width;
            var postLabel = new Label { Margin = new Thickness(20, 0, 20, 0) };
            postLabel.SetBinding(Label.FormattedTextProperty, "Post");
            var separatorLine = new BoxView { HeightRequest = 1, BackgroundColor = (Color)Application.Current.Resources["Sfondo2"], Margin = new Thickness(0, 0, 0, 0) };
            var numbersLabel = new Label { Margin = new Thickness(20, 0, 20, 0), TextColor = (Color)Application.Current.Resources["Testo3"], FontSize = 12 };
            numbersLabel.SetBinding(Label.TextProperty, "NumberOfLikes");
            var separatorLine3 = new BoxView { HeightRequest = 1, BackgroundColor = (Color)Application.Current.Resources["Sfondo2"], Margin = new Thickness(20, 0, 20, 0) };
            var playButton = new Button() { TextColor = (Color)Application.Current.Resources["App1"], FontSize = 12, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 0, 0, 0), BackgroundColor = Color.Transparent, BorderColor = Color.Transparent };
            playButton.Text = "Play";
            //playButton.SetBinding(IsEnabledProperty, "AudioExist");
            //playButton.Command = viewModel.IPlayThis;
            //playButton.SetBinding(Button.CommandParameterProperty, ".");
            var likeButton = new Button() { Text = "Like", FontSize = 12, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 0, 0, 0), BackgroundColor = Color.Transparent, BorderColor = Color.Transparent };
            //likeButton.SetBinding(Button.TextColorProperty, "Liked_me_color");
            //likeButton.Command = viewModel.ILikeThis;
            //likeButton.SetBinding(Button.CommandParameterProperty, ".");
            var commentsButton = new Button() { Text = "Comment", TextColor = (Color)Application.Current.Resources["Testo4"], FontSize = 12, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 0, 0, 0), BackgroundColor = Color.Transparent, BorderColor = Color.Transparent };
            //commentsButton.Command = viewModel.ICommentThis;
            //commentsButton.SetBinding(Button.CommandParameterProperty, ".");
            var separatorLine2 = new BoxView { HeightRequest = 1, BackgroundColor = (Color)Application.Current.Resources["Sfondo2"], Margin = new Thickness(0, 0, 0, 0) };
            var commentsLabel = new Label { Margin = new Thickness(20, 0, 20, 0), Text = "", TextColor = (Color)Application.Current.Resources["Testo3"], FontSize = 13 };
            commentsLabel.SetBinding(Label.FormattedTextProperty, "ViewComments");
            var whiteSeparator = new BoxView { BackgroundColor = (Color)Application.Current.Resources["Sfondo2"], HeightRequest = 10, Margin = new Thickness(0, 0, 0, 0) };

            //absolute layout
            var layout = new AbsoluteLayout();
            AbsoluteLayout.SetLayoutBounds(postLabel, new Rectangle(0, 10, w, 60));
            AbsoluteLayout.SetLayoutBounds(separatorLine, new Rectangle(0, 80, w, 1));
            AbsoluteLayout.SetLayoutBounds(numbersLabel, new Rectangle(0, 91, w, 28));
            AbsoluteLayout.SetLayoutBounds(separatorLine3, new Rectangle(0, 114, w, 1));
            AbsoluteLayout.SetLayoutBounds(playButton, new Rectangle(0, 109, w / 3, 48));
            AbsoluteLayout.SetLayoutBounds(likeButton, new Rectangle(w / 3, 109, w / 3, 48));
            AbsoluteLayout.SetLayoutBounds(commentsButton, new Rectangle((w / 3) * 2, 109, w / 3, 48));
            AbsoluteLayout.SetLayoutBounds(separatorLine2, new Rectangle(0, 152, w, 1));
            AbsoluteLayout.SetLayoutBounds(commentsLabel, new Rectangle(0, 157, w, 55));
            AbsoluteLayout.SetLayoutBounds(whiteSeparator, new Rectangle(0, 212, w, 10));
            layout.Children.Add(postLabel);
            layout.Children.Add(separatorLine);
            layout.Children.Add(numbersLabel);
            layout.Children.Add(separatorLine3);
            layout.Children.Add(playButton);
            layout.Children.Add(likeButton);
            layout.Children.Add(commentsButton);
            layout.Children.Add(separatorLine2);
            layout.Children.Add(commentsLabel);
            layout.Children.Add(whiteSeparator);

            View = layout;
        }
    }
}
