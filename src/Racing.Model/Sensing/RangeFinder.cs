using Racing.Mathematics;

namespace Racing.Model.Sensing
{
    internal sealed class RangeFinder
    {
        private readonly ITrack track;
        private readonly Length maximumDistance;

        public RangeFinder(ITrack track, Length maximumDistance)
        {
            this.track = track;
            this.maximumDistance = maximumDistance;
        }

        public Length DistanceToClosestObstacle(Vector origin, Angle angle)
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
