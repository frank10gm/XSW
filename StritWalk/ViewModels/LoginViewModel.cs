using System.Threading.Tasks;
using System.Windows.Input;
using System.Threading;
using System;

using Xamarin.Forms;

namespace StritWalk
{
    public class LoginViewModel : BaseViewModel
    {

        bool result;
        bool doing = false;

        public ICommand NotNowCommand { get; }
        public ICommand SignInCommand { get; }

        string message = string.Empty;
        public string Message
        {
            get { return message; }
            set { message = value; OnPropertyChanged(); }
        }


        public LoginViewModel()
        {
            SignInCommand = new Command(async () =>
            {
                if (!IsBusy && !FormIsNotReady && !doing)
                    await SignIn();
            });
            //NotNowCommand = new Command(async () => await SignUp());
        }


        async Task SignIn()
        {
            try
            {
                IsBusy = true;
                FormIsNotReady = true;
                doing = true;
                Message = "Signing In...";

                // Log the user in
                result = await TryLoginAsync();
            }
            finally
            {
                Message = string.Empty;
                IsBusy = false;

                if (!result)
                {
                    string mex = "Wrong username or password...";
                    for (var i = 0; i < mex.Length; i++)
                    {
                        Message += mex[i];
                        await Task.Delay(50);
                    }
                    await Task.Delay(500);
                    Message = string.Empty;
                }
                
                FormIsNotReady = false;
                doing = false;

                if (Settings.IsLoggedIn)
                    App.GoToMainPage();
            }
        }



        public async Task<bool> TryLoginAsync()
        {
            //await Task.Delay(1000);
            //Settings.AuthToken = "123";
            //Settings.UserId = "Frankie";
            return await DataStore.Login(Username, Password);
        }

    }
}
