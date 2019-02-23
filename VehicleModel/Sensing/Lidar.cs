using Racing.Mathematics;
using System.Collections.Generic;
using static System.Math;
using static Racing.Mathematics.CustomMath;

namespace Racing.Model.Sensing
{
    public sealed class Lidar : ILidar
    {
        private readonly ITrack track;
        private readonly Angle angularResolution;
        private readonly Length maximumDistance;

        public Lidar(ITrack track, Angle angularResolution, Length maximumDistance)
        {
            this.track = track;
            this.angularResolution = angularResolution;
            this.maximumDistance = maximumDistance;
        }

        public ILidarReading Scan(Vector origin)
        {
            var distances = new List<Length>();

            for (var angle = Angle.Zero; angle < Angle.FullCircle; angle += angularResolution)
            {
                var distance = distanceToClosestObstacle(origin, angle);
                distances.Add(distance);
            }

            return new LidarReading(
                angularResolution,
                maximumDistance,
                distances.ToArray());
        }

        private Length distanceToClosestObstacle(Vector origin, Angle angle)
        {
            var furthestVisiblePoint = origin + Vector.From(maximumDistance, angle);
            var rayPosition = origin;
            var shouldContinue = true;

            while (shouldContinue)
            {
                rayPosition = step(origin, angle, out shouldContinue);

                if (rayPosition.X >= furthestVisiblePoint.X
                    && rayPosition.Y >= furthestVisiblePoint.Y)
                {
                    rayPosition = furthestVisiblePoint;
                }
            }

            return Length.Between(origin, rayPosition);
        }

        private Vector step(Vector origin, Angle angle, out bool canContinue)
        {
            var (tileX, tileY) = track.TileOf(origin);

            var relativeX = origin.X - tileX * track.TileSize;
            var relativeY = origin.Y - tileY * track.TileSize;

            var movingUp = angle.Radians < PI;
            var movingRight = angle.Radians < PI / 2 || angle.Radians > 3 / 2 * PI;

            var remainingX = movingRight ? track.TileSize - relativeX : relativeX;
            var remainingY = movingUp ? track.TileSize - relativeY : relativeY;

            var wouldHitY = relativeY + remainingX * Tan(angle);
            var wouldHitX = relativeX + remainingY / Tan(angle);

            var exceedsHorizontally = wouldHitX < track.TileSize;
            var exceedsVertically = wouldHitY < track.TileSize;

            Vector intersection;

            if (!exceedsHorizontally && !exceedsVertically)
            {
                // hit corner
                tileX += (movingRight ? 1 : -1);
                tileY += tileY + (movingUp ? -1 : 1);
                intersection = new Vector(remainingX, remainingY);
            }
            else if (exceedsVertically)
            {
                // moved to the next cell through the top or bottom edge
                tileY += movingUp ? -1 : 1;
                intersection = new Vector(wouldHitX, remainingY);
            }
            else
            {
                // moved to the next cell through the left or right edge
                tileX += movingRight ? 1 : -1;
                intersection = new Vector(remainingX, wouldHitY);
            }

            canContinue = !track.IsOccupied(tileX, tileY);
            return intersection;
        }
    }
}
