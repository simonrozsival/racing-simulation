using Racing.Mathematics;
using Racing.Model;
using Racing.Model.Vehicle;
using System;
using static System.Math;

namespace Racing.Agents.Algorithms.Planning.RRT
{
    internal sealed class Sampler
    {
        private readonly Random random;
        private readonly ITrack track;
        private readonly IVehicleModel vehicleModel;
        private readonly IGoal goal;
        private readonly double goalBias;

        public Sampler(Random random, ITrack track, IVehicleModel vehicleModel, IGoal goal, double goalBias)
        {
            this.random = random;
            this.track = track;
            this.vehicleModel = vehicleModel;
            this.goal = goal;
            this.goalBias = goalBias;
        }

        public IState RandomSampleOfFreeRegion()
        {
            var freePosition = random.NextDouble() > goalBias ? goal.Position : randomFrePosition();
            return selectRandomSample(freePosition);
        }

        public IState selectRandomSample(Point position)
        {
            return new RandomState(
                position,
                headingAngle: random.NextDoubleBetween(0, 2 * PI),
                steeringAngle: random.NextDoubleBetween(vehicleModel.MinSteeringAngle.Radians, vehicleModel.MaxSteeringAngle.Radians),
                speed: random.NextDoubleBetween(vehicleModel.MinSpeed, vehicleModel.MaxSpeed));
        }

        private Point randomFrePosition()
        {
            Point position;
            do
            {
                position = new Point(random.NextDoubleBetween(0, track.Width), random.NextDoubleBetween(0, track.Height));
            } while (track.IsOccupied(position.X, position.Y));

            return position;
        }
    }
}
