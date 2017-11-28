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
            iBeaconService = DependencyService.Get<IIBeaconService>();
            iBeaconService.StartTracking();
        }

        async Task ScanTask()
        {
            MessagingCenter.Send(this, "NotImp", "Not Implemented");
        }



        #region PROPERTIES

        string monkeys = "MONKEYS\n\n";
        public string Monkeys
        {
            get { return monkeys; }
            set { monkeys = value; OnPropertyChanged(); }
        }


        public Command StartScanCommand { get; set; }

        #endregion

    }
}
