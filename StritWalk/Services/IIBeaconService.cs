using System;
namespace StritWalk
{
    public interface IIBeaconService
    {
        event EventHandler<GeographicLocation> LocationChanged;

        void StartTracking();

        void PauseTracking();

        void SingleTracking();
    }
}
