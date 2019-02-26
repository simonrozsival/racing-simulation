using Racing.Mathematics;
using System.Collections.Generic;

namespace Racing.Model.Sensing
{
    public sealed class Lidar : ILidar
    {
        private readonly RangeFinder rangeFinder;
        private readonly Angle fieldOfView;
        private readonly Angle angularResolution;

        public double MaximumDistance { get; }

        public Lidar(ITrack track, int samplingFrequency, Angle fieldOfView, double maximumDistance)
        {
            this.fieldOfView = fieldOfView;

            rangeFinder = new RangeFinder(track, maximumDistance);
            angularResolution = fieldOfView / samplingFrequency;

            MaximumDistance = maximumDistance;
        }

        public ILidarReading Scan(Vector origin, Angle direction)
        {
            var distances = new List<double>();
            var firstSampleDirection = direction - (fieldOfView / 2);

            for (var angle = angularResolution / 2; angle < fieldOfView; angle += angularResolution)
            {
                var distance = rangeFinder.DistanceToClosestObstacle(origin, firstSampleDirection + angle);
                distances.Add(distance);
            }

            return new LidarReading(
                angularResolution,
                firstSampleDirection,
                MaximumDistance,
                distances.ToArray());
        }
    }
}
