using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;

using Xamarin.Forms;
using Android.Gms.Common.Apis;
using Android.Gms.Common;
using Android.Gms.Location;
using Android.Util;
using Android.Widget;

[assembly: Dependency(typeof(StritWalk.Droid.IBeaconService))]

namespace StritWalk.Droid
{
    class IBeaconService : Java.Lang.Object, IIBeaconService, ILocationListener, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
    {
        LocationManager locationManager;
        GoogleApiClient apiClient;

        public event EventHandler<GeographicLocation> LocationChanged;
        public event EventHandler<string> BeaconRanged;

        public IBeaconService()
        {
            Activity activity = Toolkit.Activity;

            if (activity == null)
                throw new InvalidOperationException(
                    "Must call Toolkit.Init before using LocationProvider");

            locationManager =
                activity.GetSystemService(Context.LocationService) as LocationManager;

        }

        public void SingleTracking()
        {
            Criteria criteria = new Criteria();
            criteria.Accuracy = Accuracy.Coarse;                   
            locationManager.RequestSingleUpdate(LocationManager.NetworkProvider, this, null);
        }

        // Two methods to implement ILocationProvider (the dependency service interface).
        public void StartTracking()
        {
            IList<string> locationProviders = locationManager.AllProviders;

            foreach (string locationProvider in locationProviders)
            {
                locationManager.RequestLocationUpdates(locationProvider, 1000, 1, this);
            }
        }

        public void PauseTracking()
        {
            locationManager.RemoveUpdates(this);
        }

        // Four methods to implement ILocationListener (the Android interface).
        public void OnLocationChanged(Location location)
        {
            EventHandler<GeographicLocation> handler = LocationChanged;

            if (handler != null)
            {
                handler(this, new GeographicLocation(location.Latitude,
                                                     location.Longitude));
            }
        }

        public void OnProviderDisabled(string provider)
        {
        }

        public void OnProviderEnabled(string provider)
        {
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status,
                                    Bundle extras)
        {
        }

        public void OnConnected(Bundle connectionHint)
        {

        }

        public void OnConnectionSuspended(int cause)
        {

        }

        public void OnConnectionFailed(ConnectionResult result)
        {

        }

        public void StartTrackingBeacons()
        {
            throw new NotImplementedException();
        }

        public void PauseTrackingBeacons()
        {
            throw new NotImplementedException();
        }
    }
}