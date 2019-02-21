using Racing.Mathematics;
using Racing.Model.CollisionDetection;
using System;
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

        public IState CalculateNextState(
            IState state,
            IAction action,
            TimeSpan simulationTime,
            out bool collided)
        {
            return calculateNextState(state, action, simulationTime, null, out collided, out _);
        }

        public IState CalculateNextState(
            IState state,
            IAction action,
            TimeSpan simulationTime,
            IGoal goal,
            out bool collided,
            out bool reachedGoal)
        {
            return calculateNextState(state, action, simulationTime, goal, out collided, out reachedGoal);
        }
        private IState calculateNextState(
            IState state,
            IAction action,
            TimeSpan simulationTime,
            IGoal? goal,
            out bool collided,
            out bool reachedGoal)
        {
            collided = false;
            reachedGoal = false;

            var elapsedTime = TimeSpan.Zero;
            while (elapsedTime < simulationTime)
            {
                var step = (simulationTime - elapsedTime) < minimumSimulationTime
                    ? simulationTime - elapsedTime
                    : minimumSimulationTime;

                elapsedTime += step;

                state = calculateNextState(state, action, step);

                if (collisionDetector.IsCollision(state))
                {
                    collided = true;
                    reachedGoal = false;
                }

                if (!collided && goal != null && goal.ReachedGoal(state.Position))
                {
                    reachedGoal = true;
                }
            }

            return state;
        }

        private IState simulateUntil(
            Predicate<IState> terminationPredicate,
            IState state,
            IAction action,
            TimeSpan maxSimulationTime,
            out TimeSpan simulationTime)
        {
            simulationTime = TimeSpan.Zero;
            while (simulationTime < maxSimulationTime)
            {
                var step = (maxSimulationTime - simulationTime) < minimumSimulationTime
                    ? maxSimulationTime - simulationTime
                    : minimumSimulationTime;

                simulationTime += step;

                state = calculateNextState(state, action, step);

                if (terminationPredicate(state))
                {
                    break;
                }
            }

            return state;
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

            var velocity = new Point(
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
