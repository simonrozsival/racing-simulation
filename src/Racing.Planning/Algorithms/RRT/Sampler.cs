using Racing.Mathematics;
using Racing.Model;
using Racing.Model.Vehicle;
using System;
using static System.Math;

namespace Racing.Planning.Algorithms.RRT
{
    internal sealed class Sampler
    {
        private readonly Random random;
        private readonly ITrack track;
        private readonly IVehicleModel vehicleModel;
        private readonly double goalBias;

        public Sampler(Random random, ITrack track, IVehicleModel vehicleModel, double goalBias)
        {
            this.random = random;
            this.track = track;
            this.vehicleModel = vehicleModel;
            this.goalBias = goalBias;
        }

        public VehicleState RandomSampleOfFreeRegion(IGoal goal)
        {
            var freePosition = random.NextDouble() > goalBias
                ? goal.Position
                : randomFreePosition();

            return new VehicleState(
                freePosition,
                headingAngle: random.NextDoubleBetween(0, 2 * PI),
                angularVelocity: random.NextDoubleBetween(-vehicleModel.MaxSteeringAngle, vehicleModel.MaxSteeringAngle),
                speed: random.NextDoubleBetween(vehicleModel.MinSpeed, vehicleModel.MaxSpeed));
        }

        private Vector randomFreePosition()
        {
            Vector position;
            do
            {
                position = new Vector(
                    x: random.NextDoubleBetween(0, track.Width),
                    y: random.NextDoubleBetween(0, track.Height));
            } while (track.IsOccupied(position.X, position.Y));

            return position;
        }
    }
}
