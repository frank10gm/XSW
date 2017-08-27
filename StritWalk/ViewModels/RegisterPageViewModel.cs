using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace StritWalk
{
    public class RegisterPageViewModel : BaseViewModel
    {

        //variables
        public ICommand SignUpCommand { get; }

        //properties
        string message = string.Empty;
        public string Message
        {
            get { return message; }
            set { message = value; OnPropertyChanged(); }
        }

        public RegisterPageViewModel()
        {
            SignUpCommand = new Command(async () => await SignUp());
        }

        async Task SignUp()
        {
            try
            {
                IsBusy = true;
                Message = "Loading...";

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
                        await Task.Delay(100);
                    }
                    await Task.Delay(500);
                    Message = string.Empty;
                }


                if (Settings.IsLoggedIn)
                    App.GoToMainPage();
            }
        }
    }
}
