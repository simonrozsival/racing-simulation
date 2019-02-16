using Racing.Model.Math;
using Racing.Model.Vehicle;
using static System.Math;

namespace Racing.Model.VehicleModel
{
    internal sealed class KineticModel : IMotionModel
    {
        private readonly IVehicleModel vehicle;

        public KineticModel(IVehicleModel vehicle)
        {
            this.vehicle = vehicle;
        }

        public IState CalculateNextState(IState state, IAction action, double time)
        {
            var targetVelocity = action.Throttle * vehicle.MaxVelocity;
            var targetSteeringAngle = action.Steering * vehicle.MaxSteeringAngle;

            var maxVelocityChange = vehicle.Acceleration * time;
            var dv = Clamp(targetVelocity - state.Velocity, - maxVelocityChange, maxVelocityChange);
            var velocity = state.Velocity + dv;

            var maxSteeringChange = vehicle.SteeringAcceleration * time;
            var da = Clamp(targetSteeringAngle - state.SteeringAngle, -maxSteeringChange, maxSteeringChange);
            var steeringAngle = state.SteeringAngle + da;

            var dx = velocity * Cos(steeringAngle) * Cos(state.HeadingAngle);
            var dy = velocity * Cos(steeringAngle) * Sin(state.HeadingAngle);
            var dt = (velocity / vehicle.Length) * Sin(steeringAngle);

            return new VehicleState(
                position: new Point(
                    x: state.Position.X + dx * time,
                    y: state.Position.Y + dy * time),
                heading: toSmallAngle(state.HeadingAngle + dt * time),
                velocity: velocity,
                steering: steeringAngle);
        }

        private double toSmallAngle(double angle)
        {
            while (angle < 0) angle += 2 * PI;
            while (angle > 2 * PI) angle -= 2 * PI;
            return angle;
        }
    }
}
