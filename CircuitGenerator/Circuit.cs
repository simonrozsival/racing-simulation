using RaceCircuitGenerator.Splines;
using System.Collections.Generic;
using System.Linq;

namespace RaceCircuitGenerator
{
    internal sealed class Circuit
    {
        public double Radius { get; }
        public Point Start => WayPoints.First();
        public Point Goal => Start;
        public IList<Point> WayPoints { get; }

        public Circuit(IList<Point> waypoints, double trackWidth)
        {
            WayPoints = waypoints;
            Radius = trackWidth / 2;
        }

        public BezierCurve CenterLine()
            => new CatmullRomSpline(WayPoints).ToBezierCurve();

        public Point[] StartLine()
            => perpendicularLineTo(0);

        public IEnumerable<Point[]> WayPointLines()
            => Enumerable.Range(1, WayPoints.Count - 1).Select(perpendicularLineTo);

        private Point[] perpendicularLineTo(int i)
        {
            var point = WayPoints[i];
            var next = WayPoints[(i + 1) % WayPoints.Count];
            var prev = WayPoints[(i - 1 + WayPoints.Count) % WayPoints.Count];
            var tangentDirection = (next - prev);
            var normalVector = tangentDirection.CalculateNormal();

            var start = point + Radius * normalVector;
            var end = point - Radius * normalVector;

            return new[] { start, end };
        }
    }
}
