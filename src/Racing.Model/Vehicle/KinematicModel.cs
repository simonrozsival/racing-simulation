using Racing.Mathematics;
using System;
using System.Collections.Generic;
using static System.Math;

namespace Racing.Model.Vehicle
{
    public sealed class KinematicModel : IMotionModel
    {
        private readonly IVehicleModel vehicle;
        private readonly TimeSpan minimumSimulationTime;

        public KinematicModel(IVehicleModel vehicle, TimeSpan minimumSimulationTime)
        {
            this.vehicle = vehicle;
            this.minimumSimulationTime = minimumSimulationTime;
        }

        public IEnumerable<(TimeSpan, VehicleState)> CalculateNextState(
            VehicleState state,
            IAction action,
            TimeSpan simulationTime)
        {
            var elapsedTime = TimeSpan.Zero;
            while (elapsedTime < simulationTime)
            {
                var step = (simulationTime - elapsedTime) < minimumSimulationTime
                    ? simulationTime - elapsedTime
                    : minimumSimulationTime;

                elapsedTime += step;

                state = calculateNextState(state, action, step);

                yield return (elapsedTime, state);
            }
        }

        private VehicleState calculateNextState(VehicleState state, IAction action, TimeSpan time)
        {
            var seconds = time.TotalSeconds;

            var targetSpeed = action.Throttle * vehicle.MaxSpeed;
            var acceleration = action.Throttle > 0 ? vehicle.Acceleration : vehicle.BrakingDeceleration;
            var ds = seconds * acceleration;
            var speed = Abs(state.Speed - targetSpeed) > Abs(ds)
                ? targetSpeed // don't overshoot
                :  Clamp(state.Speed + ds, vehicle.MinSpeed, vehicle.MaxSpeed);

            var steeringAngle = action.Steering * vehicle.MaxSteeringAngle;
            var headingAngularVelocity = (state.Speed / vehicle.Length) * Sin(steeringAngle);

            var velocity = new Vector(
                x: speed * Cos(state.HeadingAngle),
                y: speed * Sin(state.HeadingAngle));
            
            return new VehicleState(
                position: state.Position + seconds * velocity,
                headingAngle: state.HeadingAngle + seconds * headingAngularVelocity,
                speed: speed,
                angularVelocity: 0);
        }
    }
}
