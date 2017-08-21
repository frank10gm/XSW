using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace StritWalk
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

		private void GoNext(object sender, EventArgs args)
		{
            PasswordField.Focus();
		}
    }
}
