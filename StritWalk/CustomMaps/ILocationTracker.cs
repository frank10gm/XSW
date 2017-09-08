using System;
namespace StritWalk
{
    public interface ILocationTracker
    {
		event EventHandler<GeographicLocation> LocationChanged;

		void StartTracking();

		void PauseTracking();

        void SingleTracking();
    }
}
