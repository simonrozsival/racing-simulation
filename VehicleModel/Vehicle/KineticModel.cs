using Racing.Mathematics;
using Racing.Model.Vehicle;
using System;
using static System.Math;

namespace Racing.Model.Vehicle
{
    public sealed class KineticModel : IMotionModel
    {
        private readonly IVehicleModel vehicle;

        public KineticModel(IVehicleModel vehicle)
        {
            this.vehicle = vehicle;
        }

        public IState CalculateNextState(IState state, IAction action, TimeSpan time)
        {
            var seconds = time.TotalSeconds;

            var targetVelocity = action.Throttle * vehicle.MaxVelocity;
            var targetSteeringAngle = action.Steering * vehicle.MaxSteeringAngle;

            var maxVelocityChange = vehicle.Acceleration * seconds;
            var dv = Clamp(targetVelocity - state.Velocity, - maxVelocityChange, maxVelocityChange);
            var velocity = state.Velocity + dv;

            var maxSteeringChange = vehicle.SteeringAcceleration * seconds;
            var da = (targetSteeringAngle - state.SteeringAngle).Clamp(-maxSteeringChange, maxSteeringChange);
            var steeringAngle = state.SteeringAngle + da;

            var velocityVector = new Point(
                x: velocity * Cos(steeringAngle.Radians) * Cos(state.HeadingAngle.Radians),
                y: velocity * Cos(steeringAngle.Radians) * Sin(state.HeadingAngle.Radians));
            Angle headingAngularVelocity = (velocity / vehicle.Length) * Sin(steeringAngle.Radians);

            return new VehicleState(
                position: state.Position + seconds * velocityVector,
                heading: state.HeadingAngle + seconds * headingAngularVelocity,
                velocity: velocity,
                steering: steeringAngle);
        }
    }
}
