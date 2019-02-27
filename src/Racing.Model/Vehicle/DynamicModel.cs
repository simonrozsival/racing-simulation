using Racing.Mathematics;
using System;
using System.Collections.Generic;
using static System.Math;

namespace Racing.Model.Vehicle
{
    public sealed class DynamicModel : IMotionModel
    {
        private readonly IVehicleModel vehicle;
        private readonly TimeSpan minimumSimulationTime;

        public DynamicModel(IVehicleModel vehicle, TimeSpan minimumSimulationTime)
        {
            this.vehicle = vehicle;
            this.minimumSimulationTime = minimumSimulationTime;
        }

        public IEnumerable<(TimeSpan, IState)> CalculateNextState(
            IState state,
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

        private IState calculateNextState(IState state, IAction action, TimeSpan time)
        {
            var seconds = time.TotalSeconds;

            var targetSpeed = action.Throttle * vehicle.MaxSpeed;
            var targetSteeringAngle = action.Steering * vehicle.MaxSteeringAngle;

            var acceleration = action.Throttle > 0
                ? Min(targetSpeed - state.Speed, vehicle.Acceleration)
                : vehicle.BrakingDeceleration;
            var steeringAcceleration = action.Steering > 0
                ? Min(targetSteeringAngle - state.SteeringAngle, vehicle.SteeringAcceleration)
                : Max(targetSteeringAngle - state.SteeringAngle, -vehicle.SteeringAcceleration);

            var ds = seconds * acceleration;
            var speed = Clamp(state.Speed + ds, vehicle.MinSpeed, vehicle.MaxSpeed);

            var da = seconds * steeringAcceleration;
            var steeringAngle = Clamp(state.SteeringAngle + da, -vehicle.MaxSteeringAngle, vehicle.MaxSteeringAngle);

            var velocity = new Vector(
                x: speed * Cos(steeringAngle) * Cos(state.HeadingAngle),
                y: speed * Cos(steeringAngle) * Sin(state.HeadingAngle));

            var headingAngularVelocity = (speed / vehicle.Length) * Sin(steeringAngle);

            return new VehicleState(
                position: state.Position + seconds * velocity,
                headingAngle: state.HeadingAngle + seconds * headingAngularVelocity,
                speed: speed,
                steeringAngle: steeringAngle);
        }
    }
}
