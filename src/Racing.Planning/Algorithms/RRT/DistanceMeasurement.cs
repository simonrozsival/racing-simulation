using Racing.Mathematics;
using Racing.Model;
using Racing.Model.Vehicle;
using static System.Math;

namespace Racing.Planning.Algorithms.RRT
{
    internal sealed class DistanceMeasurement
    {
        private double maximumDistance;

        public DistanceMeasurement(double width, double height)
        {
            var w = width;
            var h = height;
            maximumDistance = Sqrt(w * w + h * h);
        }

        public double DistanceBetween(VehicleState a, VehicleState b)
        {
            var euklideanDistance = DistanceBetween(a.Position, b.Position);
            var angleDifference = DistanceBetween(a.HeadingAngle, b.HeadingAngle);
            return euklideanDistance * euklideanDistance + angleDifference * angleDifference;
        }

        public double DistanceBetween(Vector a, Vector b)
            => (Distance.Between(a, b) / maximumDistance);

        public double DistanceBetween(double a, double b)
        {
            (a, b) = a < b ? (a, b) : (b, a);
            return Min(b - a, a + (2 * PI) - b) / (2 * PI);
        }
    }
}
