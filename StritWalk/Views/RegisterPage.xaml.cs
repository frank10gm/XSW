using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
            UserMailField.TextChanged += UserMailField_TextChanged;
            UsernameField.TextChanged += (sender, e) => verifyForm();
            PasswordField.TextChanged += (sender, e) => verifyForm();
            //SignUpButton.Clicked += (sender, e) => SignUp();
        }

        private void UserMailField_TextChanged(object sender, TextChangedEventArgs e)
        {
            const string emailRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";
            if(!Regex.IsMatch(e.NewTextValue, emailRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
            {
                if(!string.IsNullOrWhiteSpace(e.NewTextValue))
                {
                    UserMailLabel.Text = "Insert valid E-Mail address.";
                    UserMailLabel.IsVisible = true;
                }
                else
                {
                    UserMailLabel.Text = "";
                    UserMailLabel.IsVisible = false;
                }
                
            }
            else
            {
                UserMailLabel.Text = "ok";
                UserMailLabel.IsVisible = false;
                verifyForm();
            }            
        }

        private void SignUp()
        {
            vm.SignUpCommand.Execute(null);
        }

        private void verifyForm()
        {
            if (UserMailLabel.Text == "ok" && !string.IsNullOrWhiteSpace(vm.Username) && !string.IsNullOrWhiteSpace(vm.Password))
                vm.FormIsNotReady = false;
        }

    }
}
