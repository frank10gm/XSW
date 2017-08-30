using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace StritWalk
{
    public class RegisterPageViewModel : BaseViewModel
    {

        //variables
        string result = "";
        bool doing = false;

        public ICommand SignUpCommand { get; }

        string message = string.Empty;
        public string Message
        {
            get { return message; }
            set { message = value; OnPropertyChanged(); }
        }

        string email = string.Empty;
        public string Email
        {
            get => email;
            set { SetProperty(ref email, value); }
        }

        public RegisterPageViewModel()
        {
            SignUpCommand = new Command(async () =>
            {
                if (!IsBusy && !FormIsNotReady && !doing)
                    await SignUp();
            });
        }

        async Task SignUp()
        {
            try
            {
                IsBusy = true;
                doing = true;
                FormIsNotReady = true;
                Message = "Loading...";

                result = await TrySignUp();
            }
            finally
            {
                Message = string.Empty;
                IsBusy = false;

                if (result != "OK")
                {
                    string mex = result;
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

        public async Task<string> TrySignUp()
        {
            return await DataStore.SignUp(Username, Password, Email);
        }
    }
}
