using System;
using System.Collections.Generic;
using StritWalk.ViewModels;
using Xamarin.Forms;
using System.Diagnostics;

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
                Alarm();
            });
        }

        async void Alarm()
        {
            await DisplayAlert("Alert", "Function not implemented", "OK");
        }
    }
}
