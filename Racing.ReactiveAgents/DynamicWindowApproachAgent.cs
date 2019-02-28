using Racing.Mathematics;
using Racing.Model;
using Racing.Model.CollisionDetection;
using Racing.Model.Planning;
using Racing.Model.Vehicle;
using Racing.Model.Visualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using static System.Math;

namespace Racing.ReactiveAgents
{
    public sealed class DynamcWindowApproachAgent : IAgent
    {
        private readonly IReadOnlyList<IActionTrajectory> path;
        private readonly IVehicleModel vehicleModel;
        private readonly ITrack track;
        private readonly IMotionModel motionModel;
        private readonly ICollisionDetector collisionDetector;
        private readonly IActionSet actions;
        private readonly double maximumLookaheadDistance;

        private readonly TimeSpan reactionTime;

        private readonly double alignmentWeight = 5.0;
        private readonly double clearanceWeight = 0.2;
        private readonly double speedWeight = 0.5;

        private readonly ISubject<Vector> selectedLookaheadPoint = new Subject<Vector>();
        private readonly ISubject<Vector> nearestPointOnPath = new Subject<Vector>();
        private readonly ISubject<(IState[] intermediateStates, double score)> possibleActions = new Subject<(IState[], double)>();

        private IState? lastGoal = null;

        public IObservable<IVisualization> Visualization { get; }

        public DynamcWindowApproachAgent(
            IReadOnlyList<IActionTrajectory> path,
            IVehicleModel vehicleModel,
            ITrack track,
            ICollisionDetector collisionDetector,
            IMotionModel motionModel,
            IActionSet actions,
            double maximumLookaheadDistance,
            TimeSpan reactionTime)
        {
            this.path = path;
            this.vehicleModel = vehicleModel;
            this.track = track;
            this.collisionDetector = collisionDetector;
            this.motionModel = motionModel;
            this.actions = actions;
            this.maximumLookaheadDistance = maximumLookaheadDistance;
            this.reactionTime = reactionTime;

            Visualization = Observable.Merge<IVisualization>(
                Observable.Return(new Path(null, path.Select(segment => segment.State.Position).ToArray())),
                selectedLookaheadPoint.Select(point => new Dot(point, 5, "red", reactionTime)),
                nearestPointOnPath.Select(point => new Dot(point, 2, "blue", reactionTime)),
                possibleActions.Select(possibility =>
                    new ScoredPath(possibility.intermediateStates, possibility.score, reactionTime)));
        }

        public IAction ReactTo(IState state, int waypoint)
        {
            var target = findTarget(state, waypoint);
            selectedLookaheadPoint.OnNext(target);

            var bestScore = double.MinValue;
            var bestAction = actions.Brake;

            foreach (var action in actions.AllPossibleActions)
            {
                var score = calculateScoreFor(state, action, target);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestAction = action;
                }
            }

            return bestAction;
        }

        private Vector findTarget(IState currentState, int waypoint)
        {
            var nearest = 0;
            var smallestDistance = Distance.Between(currentState.Position, path[nearest].State.Position);

            for (var i = 1; i < path.Count; i++)
            {
                if (path[i].TargetWayPoint < waypoint)
                {
                    continue;
                }

                if (path[i].TargetWayPoint > waypoint + 1)
                {
                    break;
                }

                var distance = Distance.Between(currentState.Position, path[i].State.Position);
                if (distance < smallestDistance)
                {
                    nearest = i;
                    smallestDistance = distance;
                }
            }

            nearestPointOnPath.OnNext(path[nearest].State.Position);
            var lookaheadDistance = lookaheadFor(currentState);

            if (smallestDistance > lookaheadDistance)
            {
                // todo:
                lastGoal = path[nearest].State;
                return path[nearest].State.Position;
            }

            var target = nearest;
            for (var i = nearest + 1; i < path.Count; i++)
            {
                if (Distance.Between(currentState.Position, path[i].State.Position) < lookaheadDistance)
                {
                    target = i;
                }
                else
                {
                    // we previous point is as far as we can get
                    break;
                }
            }

            lastGoal = path[target].State;

            if (target == path.Count - 1)
            {
                // if the target is the last point, don't look any further
                return path[target].State.Position;
            }

            // calculate a point along the segment between the next points
            return calculateIntersection(currentState.Position, lookaheadDistance, path[target].State.Position, path[target + 1].State.Position);
        }

        private double calculateScoreFor(IState state, IAction action, Vector target)
        {
            var caution = 10;
            var predictions = motionModel.CalculateNextState(state, action, caution * reactionTime).ToArray();
            foreach (var (_, predictedState) in predictions)
            {
                if (collisionDetector.IsCollision(predictedState))
                {
                    return double.MinValue;
                }
            }

            var nextState = predictions.First().state;

            var expectedHeadingAngle = Angle.SmallAngle((target - state.Position).Direction());
            
            var headingAngle = Angle.SmallAngle(nextState.HeadingAngle);

            var headingDifference = Abs(expectedHeadingAngle - headingAngle);
            if (headingDifference > PI)
            {
                headingDifference -= PI;
            }

            var clearance = Clamp(collisionDetector.DistanceToClosestObstacle(nextState) / track.Circuit.Radius, 0, 1);
            var alignment = 1 - (Abs(headingDifference) / PI);
            var speed = nextState.Speed / vehicleModel.MaxSpeed;

            var score = clearanceWeight * clearance + alignmentWeight * alignment + speedWeight * speed;

            possibleActions.OnNext((new[] { predictions.Select(prediction => prediction.state).Last() }, score));

            return score;
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

        private double lookaheadFor(IState state)
        {
            // goal: short lookahead in corners, long on straight lines

            var straightness = lastGoal == null
                ? 1.0
                : 1.0 - Abs(lastGoal.SteeringAngle / vehicleModel.MaxSteeringAngle);

            var swiftness = state.Speed / vehicleModel.MaxSpeed;

            var lookaheadDistanceProportion = lastGoal == null
                ? 1 // we're just starting
                : Pow((straightness + swiftness) / 2, 4);

            return maximumLookaheadDistance * lookaheadDistanceProportion;
        }
    }
}
