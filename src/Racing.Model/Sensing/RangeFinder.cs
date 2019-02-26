using Racing.Mathematics;

namespace Racing.Model.Sensing
{
    internal sealed class RangeFinder
    {
        private readonly ITrack track;
        private readonly double maximumDistance;

        public RangeFinder(ITrack track, double maximumDistance)
        {
            this.track = track;
            this.maximumDistance = maximumDistance;
        }

        public double DistanceToClosestObstacle(Vector origin, double angle)
        {
            var ray = origin;
            var direction = Vector.From(track.TileSize, angle);

            while (!track.IsOccupied(ray + direction))
            {
                ray += direction;
                if (Distance.Between(origin, ray) > maximumDistance)
                {
                    return maximumDistance;
                }
            }

            return Distance.Between(origin, ray);
        }
    }
}
