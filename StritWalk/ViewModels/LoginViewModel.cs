using System.Threading.Tasks;
using System.Windows.Input;
using System.Threading;
using System;

using Xamarin.Forms;
using Com.OneSignal;
using System.Collections.Generic;

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
                //sign in if all checks are ok
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
                    //scrivere il messaggio come macchina tipografica.. non elegante
                    //for (var i = 0; i < mex.Length; i++)
                    //{
                    //    Message += mex[i];
                    //    await Task.Delay(50);
                    //}
                    Message = mex;
                    await Task.Delay(2000);
                    Message = string.Empty;
                }
                
                FormIsNotReady = false;
                doing = false;

                if (Settings.IsLoggedIn)
                {
                    //register this device for push notifications
                    OneSignal.Current.IdsAvailable(getNotifStatus);
                    App.GoToMainPage();
                }                    
            }
        }    

        public async Task<bool> TryLoginAsync()
        {
            //await Task.Delay(1000);
            //Settings.AuthToken = "123";
            //Settings.UserId = "Frankie";
            return await DataStore.Login(Username, Password);
        }

        //controlla se registrato per le notifiche
        private void getNotifStatus(string userID, string pushToken)
        {
            Settings.Notification_id = userID;
            OneSignal.Current.GetTags(getNotifTags);                   
        }

        private void getNotifTags(Dictionary<string, object> tags)
        {            
            try
            {                
                OneSignal.Current.SetSubscription(true);
                OneSignal.Current.SendTags(new Dictionary<string, string>()
                    {
                        {"UserId", Settings.AuthToken }, //purtroppo il nome della variabile è rimasto quello da inizio sviluppo... :( 
                        {"UserName", Settings.UserId }
                    });
                DataStore.addPushId(Settings.Notification_id);
                return;

                //foreach (var tag in tags)
                //    Console.WriteLine("@@@@@@@@@@@ tags : " + tag.Key + ":" + tag.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine("@@@@@@@@@@@ onesignal exception " + ex);
            }
        }

    }
}
