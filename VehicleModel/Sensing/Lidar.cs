using Racing.Mathematics;
using System.Collections.Generic;

namespace Racing.Model.Sensing
{
    public sealed class Lidar : ILidar
    {
        private readonly ITrack track;
        private readonly Angle angularResolution;
        private readonly Length maximumDistance;

        public Lidar(ITrack track, int samplingFrequency, Length maximumDistance)
        {
            this.track = track;
            this.angularResolution = Angle.FullCircle / samplingFrequency;
            this.maximumDistance = maximumDistance;
        }

        public ILidarReading Scan(Vector origin, Angle direction)
        {
            var distances = new List<Length>();

            for (var angle = Angle.Zero; angle < Angle.FullCircle; angle += angularResolution)
            {
                var distance = distanceToClosestObstacle(origin, direction + angle);
                distances.Add(distance);
            }

            return new LidarReading(
                angularResolution,
                maximumDistance,
                distances.ToArray());
        }

        private Length distanceToClosestObstacle(Vector origin, Angle angle)
        {
            var ray = origin;
            var direction = Vector.From(track.TileSize, angle);

            while (!track.IsOccupied(ray + direction))
            {
                ray += direction;
                if (Length.Between(origin, ray) > maximumDistance)
                {
                    // return Length.Infinite;
                    return maximumDistance;
                }
            }

            return Length.Between(origin, ray);
        }
    }
}
