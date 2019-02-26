using Racing.Mathematics;
using Racing.Model;
using Racing.Model.CollisionDetection;
using Racing.Model.Vehicle;
using System;
using System.Linq;

namespace Racing.ReactiveAgents
{
    public sealed class DynamcWindowApproachAgent : IAgent
    {
        private readonly IState[] path;
        private readonly IVehicleModel vehicleModel;
        private readonly ITrack track;
        private readonly IMotionModel motionModel;
        private readonly ICollisionDetector collisionDetector;
        private readonly IActionSet actions;
        private readonly Length pursuitRadius;
        private readonly TimeSpan reactionTime;

        private readonly double alignmentWeight = 2.0;
        private readonly double clearanceWeight = 0.2;
        private readonly double speedWeight = 0.2;

        public DynamcWindowApproachAgent(
            IState[] path,
            IVehicleModel vehicleModel,
            ITrack track,
            ICollisionDetector collisionDetector,
            IMotionModel motionModel,
            IActionSet actions,
            Length pursuitRadius,
            TimeSpan reactionTime)
        {
            this.path = path;
            this.vehicleModel = vehicleModel;
            this.track = track;
            this.collisionDetector = collisionDetector;
            this.motionModel = motionModel;
            this.actions = actions;
            this.pursuitRadius = pursuitRadius;
            this.reactionTime = reactionTime;
        }

        public IAction ReactTo(IState state, int waypoint)
        {
            var target = findTarget(state);

            var bestScore = double.MinValue;
            IAction bestAction = actions.Brake;

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

        private IState findTarget(IState currentState)
        {
            for (int i = path.Length; i >= 0; i++)
            {
                if (Length.Between(currentState.Position, path[i].Position) < pursuitRadius)
                {
                    return path[i];
                }
            }

            throw new Exception("Can't find any point on the path which would be within the pursuit radius.");
        }

        private double calculateScoreFor(IState state, IAction action, IState target)
        {
            var predictions = motionModel.CalculateNextState(state, action, reactionTime);
            foreach (var (_, predictedState) in predictions)
            {
                if (collisionDetector.IsCollision(predictedState))
                {
                    return double.MinValue;
                }
            }

            var nextState = predictions.Last().state;

            var headingDifference = (target.Position - nextState.Position).Direction();
            if (headingDifference > Math.PI)
            {
                headingDifference -= Math.PI;
            }

            var clearance = Math.Clamp(collisionDetector.DistanceToClosestObstacle(nextState).Meters / track.Circuit.Radius, 0, 1);
            var alignment = (Math.PI - Math.Abs(headingDifference.Radians)) / Math.PI;
            var speed = nextState.Speed / vehicleModel.MaxSpeed;

            return clearanceWeight * clearance + alignmentWeight * alignment + speedWeight * speed;
        }
    }
}
