using System;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Robotics.Mobile.Core.Bluetooth.LE;
using System.Diagnostics;
using System.Threading.Tasks;

namespace StritWalk.ViewModels
{
    public class MonkeyViewModel : ObservableObject
    {

        IIBeaconService iBeaconService;

        public MonkeyViewModel()
        {            

            StartScanCommand = new Command(async () => await ScanTask());

            if (Xamarin.Forms.Device.iOS == Xamarin.Forms.Device.RuntimePlatform)
            {
                iBeaconService = DependencyService.Get<IIBeaconService>();
                iBeaconService.StartTracking();
                iBeaconService.LocationChanged += (sender, e) =>
                {
                    if (e != "no")
                        Monkeys = "There is a SCR " + e + "\n\n" + Settings.LastBea;
                    else
                        Monkeys = "There are no SCRs here\n\n" + "" + Settings.LastBea;
                };
            }
        }

        async Task ScanTask()
        {
            MessagingCenter.Send(this, "NotImp", "Not Implemented");

            await Task.Run(() =>
            {
                
            });
        }



        #region PROPERTIES

        string monkeys = "There are no SCRs here\n\n" + "" + Settings.LastBea;
        public string Monkeys
        {
            get { return monkeys; }
            set { monkeys = value; OnPropertyChanged(); }
        }


        public Command StartScanCommand { get; set; }

        #endregion

    }
}
