using Racing.Mathematics;
using Racing.Mathematics.Splines;
using Racing.Model;
using System.Collections.Generic;
using System.Linq;

namespace Racing.CircuitGenerator
{
    internal sealed class Circuit : ICircuit
    {
        public double Radius { get; }
        public Point Start => WayPoints.First().Position;
        public IList<IGoal> WayPoints { get; }

        public Circuit(IList<Point> waypoints, double trackWidth)
        {
            Radius = trackWidth / 2;
            WayPoints = waypoints.Select(point => new RadialGoal(point, Radius)).ToList<IGoal>();
        }

        public BezierCurve CenterLine()
            => new CatmullRomSpline(WayPoints.Select(goal => goal.Position)).ToBezierCurve();

        public Point[] StartLine()
            => perpendicularLineTo(0);

        public IEnumerable<Point[]> WayPointLines()
            => Enumerable.Range(1, WayPoints.Count - 1).Select(perpendicularLineTo);

        private Point[] perpendicularLineTo(int i)
        {
            var point = WayPoints[i].Position;
            var next = WayPoints[(i + 1) % WayPoints.Count].Position;
            var prev = WayPoints[(i - 1 + WayPoints.Count) % WayPoints.Count].Position;
            var tangentDirection = (next - prev);
            var normalVector = tangentDirection.CalculateNormal();

            var start = point + Radius * normalVector;
            var end = point - Radius * normalVector;

            return new[] { start, end };
        }
    }
}
