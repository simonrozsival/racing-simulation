using Racing.Mathematics;
using Racing.Model;
using Racing.Model.Vehicle;
using System;
using System.Collections.Generic;
using static System.Math;

namespace Racing.Agents.Algorithms.Planning.RRT
{
    internal sealed class Sampler
    {
        private readonly Random random;
        private readonly ITrack track;
        private readonly IVehicleModel vehicleModel;
        private readonly IReadOnlyList<IGoal> wayPoints;
        private readonly double goalBias;

        public Sampler(Random random, ITrack track, IVehicleModel vehicleModel, IReadOnlyList<IGoal> wayPoints, double goalBias)
        {
            this.random = random;
            this.track = track;
            this.vehicleModel = vehicleModel;
            this.wayPoints = wayPoints;
            this.goalBias = goalBias;
        }

        public IState RandomSampleOfFreeRegion(int currentGoal)
        {
            var freePosition = random.NextDouble() > goalBias
                ? wayPoints[currentGoal].Position
                : randomFreePosition();

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

        private Point randomFreePosition()
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
