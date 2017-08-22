using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace StritWalk
{
    public partial class RegisterPage : ContentPage
    {
        RegisterPageViewModel vm;

        public RegisterPage(RegisterPageViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.vm = viewModel;


        }

    }
}
