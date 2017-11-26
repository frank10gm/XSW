using System;
using System.Collections.Generic;
using UniversalBeacon.Library.Core.Entities;
using UniversalBeacon.Library.Core.Interop;
using UniversalBeacon.Library.Core.Interfaces;
using UniversalBeacon.Library;
using Xamarin.Forms;
using OpenNETCF.IoC;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace StritWalk.Models
{
    public class BeaconService : IDisposable
    {

        private readonly BeaconManager _manager;

        public BeaconService()
        {
            var provider = RootWorkItem.Services.Get<IBluetoothPacketProvider>();
            if(provider != null){
                _manager = new BeaconManager(provider, Device.BeginInvokeOnMainThread);
                _manager.Start();    
            }
        }

        public void Dispose()
        {
            _manager?.Stop();
        }

        public ObservableCollection<Beacon> Beacons => _manager.BluetoothBeacons;
    }
}
