using System;
namespace StritWalk
{
    public interface IIBeaconService
    {
        event EventHandler<string> LocationChanged;

        void StartTracking();

        void PauseTracking();

        void SingleTracking();
    }
}
