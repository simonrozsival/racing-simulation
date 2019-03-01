using Racing.Mathematics;
using Racing.Model.Planning;
using Racing.Model.Vehicle;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace Racing.Model.Actions
{
    public sealed class Trajectory
    {
        public IReadOnlyList<IActionTrajectory> Segments { get; }

        public Trajectory(IReadOnlyList<IActionTrajectory> actionTrajectory)
        {
            if (actionTrajectory.Count == 0)
            {
                throw new ArgumentException("Action trajectory can't be empty.");
            }

            this.Segments = actionTrajectory;
        }

        public VehicleState FindProjection(VehicleState state, int waypoint)
        {
            var nearest = Segments.First().State;
            var smallestDistance = Distance.Between(state.Position, nearest.Position);

            foreach (var segment in Segments)
            {
                if (segment.TargetWayPoint < waypoint)
                {
                    continue;
                }

                if (segment.TargetWayPoint > waypoint + 1)
                {
                    break;
                }

                var distance = Distance.Between(state.Position, segment.State.Position);
                if (distance < smallestDistance)
                {
                    nearest = segment.State;
                    smallestDistance = distance;
                }
            }

            return nearest;
        }

        public VehicleState FindTarget(VehicleState state, int waypoint, double lookahead)
        {
            var lastSegment = Segments.Count - 1;
            var target = lastSegment;
            var orderedCandidates = Segments.Where(segment => segment.TargetWayPoint == waypoint).Reverse();
            for (var i = lastSegment; i >= 0; i--)
            {
                if (Distance.Between(state.Position, Segments[i].State.Position) < lookahead)
                {
                    target = i;
                }
                else
                {
                    // we previous point is as far as we can get
                    break;
                }
            }

            if (target == lastSegment)
            {
                // if the target is the last point, don't look any further
                return Segments[lastSegment].State;
            }

            // calculate a point along the segment between the next points
            return interpolate(state, Segments[target].State, Segments[target + 1].State, lookahead);
        }

        private VehicleState interpolate(VehicleState state, VehicleState target, VehicleState afterTarget, double lookahead)
        {
            var intermediatePosition = calculateIntersection(state.Position, lookahead, target.Position, afterTarget.Position);
            var t = Distance.Between(intermediatePosition, target.Position) / Distance.Between(afterTarget.Position, target.Position);

            return new VehicleState(
                position: intermediatePosition,
                headingAngle: t * target.HeadingAngle + (1 - t) * afterTarget.HeadingAngle,
                angularVelocity: t * target.AngularVelocity + (1 - t) * afterTarget.AngularVelocity,
                speed: t * target.Speed + (1 - t) * afterTarget.Speed);
        }

        /// <summary>
        /// Find a point along the segment defined by "B" and "C"
        /// "B" is inside of the circle with center "A" and radius "r", "C" is outside of the circle.
        /// </summary>
        /// <param name="C">Center of a circle.</param>
        /// <param name="r">Radius of the circle</param>
        /// <param name="A">Start point of the segment which lies inside of the circle.</param>
        /// <param name="B">End point of the segment which lies outside of the circle.</param>
        /// <returns>Point which lies on the intersection of the circle and the segment.</returns>
        private Vector calculateIntersection(Vector C, double r, Vector A, Vector B)
        {
            var d = B - A; // direction of the segment
            var f = A - C; // center to start

            double square(double x) => x * x;

            var a = d.Dot(d);
            var b = 2 * f.Dot(d);
            var c = f.Dot(f) - square(r);

            var D = square(b) - 4 * a * c;

            if (D < 0)
            {
                // the equation has no solution 😱
                throw new ArgumentException(
                    $"The configuration k({C}, {r}), A={A}, B={B} doesn't meet the requirements " +
                    $"(the segment AB doesn't intersect with the circle k).");
            }

            var t1 = (-b + Sqrt(D)) / (2 * a);
            //var t2 = (-b - Sqrt(D)) / (2 * a); -- we only need the positive value

            if (t1 < 0 || t1 > 1)
            {
                throw new Exception($"Can't calculate the intersection between k({C}, {r}) and segment AB(A={A}, B={B}).");
            }

            return A + t1 * d;
        }
    }
}
