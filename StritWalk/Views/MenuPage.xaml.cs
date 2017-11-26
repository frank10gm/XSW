using System;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Robotics.Mobile.Core.Bluetooth.LE;
using System.Diagnostics;
using System.Threading.Tasks;

namespace StritWalk
{
    public partial class MenuPage : ContentPage
    {
        public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();

        IAdapter adapter;
        ObservableCollection<IDevice> devices;

        public MenuPage(IAdapter adapter)
        {
            InitializeComponent();

            this.adapter = adapter;
            this.devices = new ObservableCollection<IDevice>();

            adapter.DeviceDiscovered += (object sender, DeviceDiscoveredEventArgs e) => {
                Debug.WriteLine(e.Device.Name + " : " + e.Device.ID);
                BeaLabel.Text += e.Device.Name + " : " + e.Device + "\n";
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                    devices.Add(e.Device);
                });
            };

            adapter.ScanTimeoutElapsed += (sender, e) => {
                //adapter.StopScanningForDevices(); // not sure why it doesn't stop already, if the timeout elapses... or is this a fake timeout we made?
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                    //DisplayAlert("Timeout", "Bluetooth scan timeout elapsed", "OK");
                });
            };
        }

        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            await DataStore.removePushId();
            App.LogOut();        
        }

        void StartScanning()
        {
            StartScanning(Guid.Empty);
        }
        void StartScanning(Guid forService)
        {
            Console.WriteLine("start scanning");
            if (adapter.IsScanning)
            {
                adapter.StopScanningForDevices();
                Debug.WriteLine("adapter.StopScanningForDevices()");
            }
            else
            {
                devices.Clear();
                adapter.StartScanningForDevices(forService);
                Debug.WriteLine("adapter.StartScanningForDevices(" + forService + ")");
            }
        }

        void StopScanning()
        {            
            BeaLabel.Text = "";
            // stop scanning
            new Task(() => {
                if (adapter.IsScanning)
                {
                    Debug.WriteLine("Still scanning, stopping the scan");
                    adapter.StopScanningForDevices();

                }
            }).Start();
        }

        void Handle_Clicked2(object sender, System.EventArgs e)
        {
            StartScanning();
        }
    }
}
