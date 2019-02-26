using Racing.Mathematics;
using System.Collections.Generic;

namespace Racing.Model.Sensing
{
    public sealed class Lidar : ILidar
    {
        private readonly RangeFinder rangeFinder;
        private readonly double fieldOfView;
        private readonly double angularResolution;

        public double MaximumDistance { get; }

        public Lidar(ITrack track, int samplingFrequency, double fieldOfView, double maximumDistance)
        {
            this.fieldOfView = fieldOfView;

            rangeFinder = new RangeFinder(track, maximumDistance);
            angularResolution = fieldOfView / samplingFrequency;

            MaximumDistance = maximumDistance;
        }

        public ILidarReading Scan(Vector origin, double headingAngle)
        {
            var distances = new List<double>();
            var firstSampleDirection = headingAngle - (fieldOfView / 2);

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
