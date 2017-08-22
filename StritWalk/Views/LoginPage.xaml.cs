using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace StritWalk
{
    public partial class LoginPage : ContentPage
    {

        LoginViewModel vm;

        public LoginPage()
        {
            InitializeComponent();
            vm = BindingContext as LoginViewModel;
        }

		private void GoNext(object sender, EventArgs args)
		{
            PasswordField.Focus();
		}

        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            Console.WriteLine("prova");
            Console.WriteLine(e.NewTextValue);
        }

        void Handle_Completed(object sender, System.EventArgs e)
        {
            vm.SignInCommand.Execute(null);
        }
    }
}
