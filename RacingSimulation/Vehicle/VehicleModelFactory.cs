using Racing.Model;
using Racing.Model.Vehicle;

namespace Racing.Simulation.Vehicle
{
    internal static class VehicleModelFactory
    {
        public static IVehicleModel ForwardDrivingOnlyWhichFitsOnto(ITrack track)
        {
            var width = track.Circuit.Radius / 3; // the width of the track is 6 widths of the car
            return new ForwardDrivingOnlyVehicle(width);
        }
    }
}
