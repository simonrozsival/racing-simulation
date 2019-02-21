using Racing.Mathematics;
using Racing.Model;
using static System.Math;

namespace Racing.Agents.Algorithms.Planning.RRT
{
    internal sealed class DistanceMeasurement
    {
        private double distanceFromStartToGoalSquared;

        public DistanceMeasurement(Point start, Point goal)
        {
            distanceFromStartToGoalSquared = start.DistanceSq(goal);
        }

        public double DistanceBetween(IState a, IState b)
        {
            var euklideanDistance = DistanceBetween(a.Position, b.Position);
            var angleDifference = DistanceBetween(a.HeadingAngle, b.HeadingAngle);
            return euklideanDistance * euklideanDistance + angleDifference * angleDifference;
        }

        public double DistanceBetween(Point a, Point b)
            => a.DistanceSq(b) / distanceFromStartToGoalSquared;

        public double DistanceBetween(Angle a, Angle b)
        {
            (a, b) = a.Radians < b.Radians ? (a, b) : (b, a);
            return Min(b.Radians - a.Radians, a.Radians + (2 * PI) - b.Radians) / (2 * PI);
        }
    }
}
