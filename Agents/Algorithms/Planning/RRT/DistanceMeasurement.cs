using Racing.Mathematics;
using Racing.Model;
using static System.Math;

namespace Racing.Agents.Algorithms.Planning.RRT
{
    internal sealed class DistanceMeasurement
    {
        private double maximumDistance;

        public DistanceMeasurement(double width, double height)
        {
            maximumDistance = Sqrt(width * width + height * height);
        }

        public double DistanceBetween(IState a, IState b)
        {
            var euklideanDistance = DistanceBetween(a.Position, b.Position);
            var angleDifference = DistanceBetween(a.HeadingAngle, b.HeadingAngle);
            return euklideanDistance * euklideanDistance + angleDifference * angleDifference;
        }

        public double DistanceBetween(Point a, Point b)
            => a.DistanceSq(b) / maximumDistance;

        public double DistanceBetween(Angle a, Angle b)
        {
            (a, b) = a.Radians < b.Radians ? (a, b) : (b, a);
            return Min(b.Radians - a.Radians, a.Radians + (2 * PI) - b.Radians) / (2 * PI);
        }
    }
}
