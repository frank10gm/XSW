using System.Threading.Tasks;
using System.Windows.Input;
using System.Threading;
using System;

using Xamarin.Forms;

namespace StritWalk
{
    public class LoginViewModel : BaseViewModel
    {
        public LoginViewModel()
        {
            SignInCommand = new Command(async () => await SignIn());
            NotNowCommand = new Command(async () => await SignUp());
        }

        string message = string.Empty;
        public string Message
        {
            get { return message; }
            set { message = value; OnPropertyChanged(); }
        }

        public ICommand NotNowCommand { get; }
        public ICommand SignInCommand { get; }

        bool result;

        async Task SignIn()
        {
            try
            {
                IsBusy = true;
                Message = "Signing In...";

                // Log the user in
                result = await TryLoginAsync();
            }
            finally
            {
                Message = string.Empty;
                IsBusy = false;
                //if(result)
                //App.GoToMainPage();

                if (Settings.IsLoggedIn)
                    App.GoToMainPage();
            }
        }

        async Task SignUp()
        {
            try
            {
                IsBusy = true;
            }
            finally
            {

            }
        }

        public static async Task<bool> TryLoginAsync()
        {
            await Task.Delay(1000);
            Settings.AuthToken = "123";
            Settings.UserId = "Frankie";
            return false;
        }
    }
}
