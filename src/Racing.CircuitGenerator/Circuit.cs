using Racing.Mathematics;
using Racing.Mathematics.Splines;
using Racing.Model;
using Racing.Model.Vehicle;
using System.Collections.Generic;
using System.Linq;

namespace Racing.CircuitGenerator
{
    internal sealed class Circuit : ICircuit
    {
        public double Radius { get; }
        public Vector Start => WayPoints.First().Position;
        public IList<IGoal> WayPoints { get; }
        public VehicleState StartingPosition =>
            new VehicleState(
                position: Start,
                headingAngle: tangent(0).Direction(),
                steeringAngle: 0,
                speed: 0);

        public Circuit(IList<Vector> waypoints, double trackWidth)
        {
            Radius = trackWidth / 2;
            WayPoints = waypoints.Select(point => new RadialGoal(point, Radius)).ToList<IGoal>();
        }

        public BezierCurve CenterLine()
            => new CatmullRomSpline(WayPoints.Select(goal => goal.Position)).ToBezierCurve();

        public Vector[] StartLine()
            => perpendicularLineTo(0);

        public IEnumerable<Vector[]> WayPointLines()
            => Enumerable.Range(1, WayPoints.Count - 1).Select(perpendicularLineTo);

        private Vector tangent(int i)
        {
            var next = WayPoints[(i + 1) % WayPoints.Count].Position;
            var prev = WayPoints[(i - 1 + WayPoints.Count) % WayPoints.Count].Position;
            return next - prev;
        }

        private Vector[] perpendicularLineTo(int i)
        {
            var point = WayPoints[i].Position;
            var normalVector = tangent(i).CalculateNormal();

            var start = point + Radius * normalVector;
            var end = point - Radius * normalVector;

            return new[] { start, end };
        }
    }
}
