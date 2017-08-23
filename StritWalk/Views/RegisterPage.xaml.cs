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

            UserMailField.Completed += (sender, e) => UsernameField.Focus();
            UsernameField.Completed += (sender, e) => PasswordField.Focus();
            PasswordField.Completed += (sender, e) => SignUp();
            SignUpButton.Clicked += (sender, e) => SignUp();
        }

        private void SignUp()
        {
            Console.WriteLine("sign up method");
        }

    }
}
