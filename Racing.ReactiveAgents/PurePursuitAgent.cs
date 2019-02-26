using Racing.Mathematics;
using Racing.Model;
using Racing.Model.Vehicle;
using System;

namespace Racing.ReactiveAgents
{
    public sealed class PurePursuitAgent : IAgent
    {
        private readonly IState[] path;
        private readonly IVehicleModel vehicleModel;
        private readonly Length pursuitRadius;
        private readonly PIDController steeringController;

        public PurePursuitAgent(IState[] path, IVehicleModel vehicleModel, Length pursuitRadius)
        {
            this.path = path;
            this.vehicleModel = vehicleModel;
            this.pursuitRadius = pursuitRadius;
            this.steeringController = new PIDController(1, 0, 0);
        }

        public IAction ReactTo(IState state, int waypoint)
        {
            var target = findTarget(state);

            var throttle = target.Speed / vehicleModel.MaxSpeed;
            var steering = calculateSteering(state, target);

            return new SeekAction(throttle, steering);
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

        private double calculateSteering(IState currentState, IState target)
        {
            var currentAngle = currentState.HeadingAngle;
            var targetAngle = (target.Position - currentState.Position).Direction();
            var difference = (targetAngle - currentAngle).Radians;
            
            if (difference > Math.PI)
            {
                difference -= Math.PI;
            }
            if (difference < -Math.PI)
            {
                difference += Math.PI;
            }

            difference /= vehicleModel.MaxSteeringAngle.Radians;

            var steering = steeringController.CalculateAction(target: 0, difference);

            return Math.Clamp(steering, -1, 1);
        }
    }
}
