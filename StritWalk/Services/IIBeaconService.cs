using System;
namespace StritWalk
{
    public interface IIBeaconService
    {
        event EventHandler<GeographicLocation> LocationChanged;
        event EventHandler<string> BeaconRanged;

        void StartTracking();

        void PauseTracking();

        void StartTrackingBeacons();

        void PauseTrackingBeacons();

        void SingleTracking();
    }
}
