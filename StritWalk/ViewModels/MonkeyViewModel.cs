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
                        Monkeys = "There is a SCRAMBLER " + e;
                };
            }
        }

        async Task ScanTask()
        {
            await Task.Run(() =>
            {
                MessagingCenter.Send(this, "NotImp", "Not Implemented");
            });
        }



        #region PROPERTIES

        string monkeys = "There are no SCRAMBLERs here\n\n";
        public string Monkeys
        {
            get { return monkeys; }
            set { monkeys = value; OnPropertyChanged(); }
        }


        public Command StartScanCommand { get; set; }

        #endregion

    }
}
