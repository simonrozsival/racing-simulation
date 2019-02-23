using Racing.Mathematics;
using Racing.Model.CollisionDetection;
using System;
using System.Collections.Generic;
using static System.Math;

namespace Racing.Model.Vehicle
{
    public sealed class DynamicModel : IMotionModel
    {
        private readonly IVehicleModel vehicle;
        private readonly ICollisionDetector collisionDetector;
        private readonly TimeSpan minimumSimulationTime;

        public DynamicModel(IVehicleModel vehicle, ICollisionDetector collisionDetector, TimeSpan minimumSimulationTime)
        {
            this.vehicle = vehicle;
            this.collisionDetector = collisionDetector;
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

            var acceleration = action.Throttle * vehicle.Acceleration;
            var steeringAcceleration = action.Steering * vehicle.SteeringAcceleration;

            var ds = seconds * acceleration;
            var speed = Clamp(state.Speed + ds, vehicle.MinSpeed, vehicle.MaxSpeed);

            var da = seconds * steeringAcceleration.Radians;
            var steeringAngle = Clamp(state.SteeringAngle.Radians + da, vehicle.MinSteeringAngle.Radians, vehicle.MaxSteeringAngle.Radians);

            var velocity = new Vector(
                x: speed * Cos(steeringAngle) * Cos(state.HeadingAngle.Radians),
                y: speed * Cos(steeringAngle) * Sin(state.HeadingAngle.Radians));

            Angle headingAngularVelocity = (speed / vehicle.Length) * Sin(steeringAngle);

            return new VehicleState(
                position: state.Position + seconds * velocity,
                headingAngle: state.HeadingAngle + seconds * headingAngularVelocity,
                speed: speed,
                steeringAngle: steeringAngle);
        }
    }
}
