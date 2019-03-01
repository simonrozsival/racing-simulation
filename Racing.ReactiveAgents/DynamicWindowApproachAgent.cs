using Racing.Mathematics;
using Racing.Model;
using Racing.Model.Actions;
using Racing.Model.CollisionDetection;
using Racing.Model.Vehicle;
using Racing.Model.Visualization;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using static System.Math;

namespace Racing.ReactiveAgents
{
    public sealed class DynamcWindowApproachAgent : IAgent
    {
        private readonly Trajectory trajectory;
        private readonly IVehicleModel vehicleModel;
        private readonly ITrack track;
        private readonly IMotionModel motionModel;
        private readonly ICollisionDetector collisionDetector;
        private readonly IActionSet actions;
        private readonly double maximumLookaheadDistance;

        private readonly TimeSpan predictionHorizon;

        private readonly double alignmentWeight = 5.0;
        private readonly double clearanceWeight = 0.2;
        private readonly double speedWeight = 0.5;

        private readonly ISubject<Vector> selectedLookaheadPoint = new Subject<Vector>();
        private readonly ISubject<Vector> nearestPointOnPath = new Subject<Vector>();
        private readonly ISubject<(VehicleState[] intermediateStates, double score)> possibleActions = new Subject<(VehicleState[], double)>();

        private VehicleState? lastGoal = null;

        public IObservable<IVisualization> Visualization { get; }

        public DynamcWindowApproachAgent(
            Trajectory trajectory,
            IWorldDefinition world,
            double maximumLookaheadDistance,
            TimeSpan reactionTime,
            TimeSpan predictionHorizon)
        {
            this.trajectory = trajectory;
            this.maximumLookaheadDistance = maximumLookaheadDistance;
            this.predictionHorizon = predictionHorizon;

            vehicleModel = world.VehicleModel;
            track = world.Track;
            collisionDetector = world.CollisionDetector;
            motionModel = world.MotionModel;
            actions = world.Actions;

            Visualization = Observable.Merge<IVisualization>(
                Observable.Return(new Path(null, trajectory.Segments.Select(segment => segment.State.Position).ToArray())),
                selectedLookaheadPoint.Select(point => new Dot(point, 5, "red", reactionTime)),
                nearestPointOnPath.Select(point => new Dot(point, 2, "blue", reactionTime)),
                possibleActions.Select(possibility =>
                    new ScoredPath(possibility.intermediateStates, possibility.score, reactionTime)));
        }

        public IAction ReactTo(VehicleState state, int waypoint)
        {
            var furthestPossibleTarget = trajectory.FindTarget(state, waypoint, maximumLookaheadDistance);
            var lookahead = lookaheadFor(furthestPossibleTarget);

            var target = lookahead == maximumLookaheadDistance
                ? furthestPossibleTarget
                : trajectory.FindTarget(state, waypoint, lookahead);

            selectedLookaheadPoint.OnNext(target.Position);

            var bestScore = double.MinValue;
            var bestAction = actions.Brake;

            foreach (var action in actions.AllPossibleActions)
            {
                var score = calculateScoreFor(state, action, target.Position);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestAction = action;
                }
            }

            return bestAction;
        }

        private double calculateScoreFor(VehicleState state, IAction action, Vector target)
        {
            var predictions = motionModel.CalculateNextState(state, action, predictionHorizon).ToArray();
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

        private double lookaheadFor(VehicleState state)
        {
            // goal: short lookahead in corners, long on straight lines

            var straightness = !lastGoal.HasValue
                ? 1.0
                : 1.0 - Abs(lastGoal.Value.HeadingAngle / vehicleModel.MaxSteeringAngle);

            var swiftness = state.Speed / vehicleModel.MaxSpeed;

            var lookaheadDistanceProportion = lastGoal == null
                ? 1 // we're just starting
                : Pow((straightness + swiftness) / 2, 4);

            return maximumLookaheadDistance * lookaheadDistanceProportion;
        }
    }
}
