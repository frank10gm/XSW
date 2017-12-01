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
        
        //variables / properties
        IIBeaconService iBeaconService;

        string monkeys = "There are no SCRs here\n\n" + "" + Settings.LastBea;
        public string Monkeys
        {
            get { return monkeys; }
            set { monkeys = value; OnPropertyChanged(); }
        }

        public Command StartScanCommand { get; set; }
        public Command StopScanCommand { get; set; }


        //constructor
        public MonkeyViewModel()
        {
            StartScanCommand = new Command(async () => await ScanTask());
            StopScanCommand = new Command(StopScan);

            if (Xamarin.Forms.Device.iOS == Xamarin.Forms.Device.RuntimePlatform)
            {
                iBeaconService = DependencyService.Get<IIBeaconService>();

                iBeaconService.BeaconRanged += (sender, e) =>
                {
                    if (e != "no")
                        Monkeys = "There is a SCR " + e + "\n\n" + Settings.LastBea;
                    else
                        Monkeys = "There are no SCRs here\n\n" + "" + Settings.LastBea;
                };
            }
        }


        //functions
        async Task ScanTask()
        {
            //MessagingCenter.Send(this, "NotImp", "Not Implemented");

            if (Xamarin.Forms.Device.iOS == Xamarin.Forms.Device.RuntimePlatform)
                iBeaconService.StartTrackingBeacons();    

            await Task.Run(() =>
            {

            });
        }

        void StopScan(){
            if (Xamarin.Forms.Device.iOS == Xamarin.Forms.Device.RuntimePlatform)
                iBeaconService.PauseTrackingBeacons();
        }


    }
}
