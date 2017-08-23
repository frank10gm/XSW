using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace StritWalk
{
    public partial class LoginPage : ContentPage
    {
        LoginViewModel vm;
        public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();

        public LoginPage()
        {
            InitializeComponent();
            vm = BindingContext as LoginViewModel;
            vm.Navigation = Navigation;
        }

		private void GoNext(object sender, EventArgs args)
		{
            PasswordField.Focus();
		}

        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            
        }

        void Handle_Completed(object sender, System.EventArgs e)
        {
            vm.SignInCommand.Execute(null);
        }

        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage(new RegisterPageViewModel()));
        }
    }
}
