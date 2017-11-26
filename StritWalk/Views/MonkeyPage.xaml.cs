using System;
using System.Collections.Generic;
using StritWalk.ViewModels;
using Xamarin.Forms;

namespace StritWalk.Views
{
    public partial class MonkeyPage : ContentPage
    {

        MonkeyViewModel viewModel;

        public MonkeyPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new MonkeyViewModel();

            MessagingCenter.Subscribe<MonkeyViewModel, string>(this, "NotImp", (sender, arg) =>
            {
                DisplayAlert("Alert", "Function not implemented", "OK");
            });
        }
    }
}
