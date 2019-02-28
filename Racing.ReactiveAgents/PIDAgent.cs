using Racing.Mathematics;
using Racing.Model;
using Racing.Model.Planning;
using Racing.Model.Vehicle;
using Racing.Model.Visualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Racing.ReactiveAgents
{
    public sealed class PIDAgent : IAgent
    {
        private readonly IReadOnlyList<IActionTrajectory> path;
        private readonly IMotionModel motionModel;
        private readonly TimeSpan reactionTime;
        private readonly PIDController steeringController;

        private readonly ISubject<Vector> projectedPosition = new Subject<Vector>();
        private readonly ISubject<Vector> selectedTarget = new Subject<Vector>();
        private readonly ISubject<double> crossTrackError = new Subject<double>();

        private IAction? lastAction = null;

        public IObservable<IVisualization> Visualization { get; }

        public PIDAgent(IReadOnlyList<IActionTrajectory> path, IMotionModel motionModel, TimeSpan reactionTime)
        {
            this.path = path;
            this.motionModel = motionModel;
            this.reactionTime = reactionTime;

            steeringController = new PIDController(0.0395, 0.000155, -0.785);

            Visualization = Observable.Merge<IVisualization>(
                Observable.Return(new Path(null, path.Select(segment => segment.State.Position).ToArray())),
                selectedTarget.Select(point => new Dot(point, 2, "red", reactionTime)),
                projectedPosition.Select(point => new Dot(point, 2, "blue", reactionTime)),
                crossTrackError.WithLatestFrom(selectedTarget, (error, target) => new Dot(target, Math.Abs(error), Math.Sign(error) == 1 ? "blue" : "red", reactionTime)));
        }

        public IAction ReactTo(IState state, int waypoint)
        {
            var projectedState = lastAction == null
                ? state
                : motionModel.CalculateNextState(state, lastAction, reactionTime).Last().state;
            projectedPosition.OnNext(projectedState.Position);

            var target = findClosestPoint(projectedState);
            selectedTarget.OnNext(path[target].State.Position);

            var throttle = 0.1; // target.Speed / vehicleModel.MaxSpeed;
            var steering = calculateSteering(state, target);

            var action = new SeekAction(throttle, steering);
            lastAction = action;
            return action;
        }

        private int findClosestPoint(IState state)
        {
            var closestState = 0;
            var shortestDistance = double.MaxValue;

            for (int i = 0; i < path.Count; i++)
            {
                var segment = path[i];
                var distance = Distance.Between(state.Position, segment.State.Position);
                if (distance < shortestDistance)
                {
                    closestState = i;
                    shortestDistance = distance;
                }
            }

            return closestState;
        }

        private double calculateSteering(IState currentState, int target)
        {
            var targetPosition = path[target].State.Position;
            var followingPointPosition = target < path.Count - 1
                ? path[target + 1].State.Position
                : targetPosition + (targetPosition - path[target - 1].State.Position); // extend the last segment in the same direction
            var targetDirection = followingPointPosition - targetPosition;
            var directionToTarget = targetPosition - currentState.Position;
            var leftOrRight = Math.Sign(targetDirection.Cross(directionToTarget));

            var distance = Distance.Between(targetPosition, currentState.Position);

            crossTrackError.OnNext(leftOrRight * distance);

            var steering = -steeringController.Calculate(target: 0, leftOrRight * distance);

            Console.WriteLine($"cte={leftOrRight * distance}, steering={steering}");

            return Math.Clamp(steering, -1, 1);
        }
    }
}
