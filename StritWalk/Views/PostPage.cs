using System;

using Xamarin.Forms;

namespace StritWalk.Views
{
    public class PostPage : ContentPage
    {
        public PostPage()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Hello ContentPage" }
                }
            };
        }
    }
}

