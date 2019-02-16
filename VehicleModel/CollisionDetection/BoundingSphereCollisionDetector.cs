using Racing.Model.Vehicle;
using System;

namespace Racing.Model.CollisionDetection
{
    public sealed class BoundingSphereCollisionDetector : ICollisionDetector
    {
        private readonly ITrack track;
        private readonly double diagonal;
        private readonly BoundsDetector boundsDetector;

        public BoundingSphereCollisionDetector(ITrack track, IVehicleModel vehicleModel)
        {
            this.track = track;

            boundsDetector = new BoundsDetector(track);

            var u = vehicleModel.Length / 2;
            var v = vehicleModel.Width / 2;
            diagonal = Math.Sqrt(u * u + v * v);
        }

        public bool IsCollision(IState state)
        {
            for (var dx = -1; dx <= 1; dx++)
            {
                for (var dy = -1; dy <= 1; dy++)
                {
                    var x = state.Position.X + dx * diagonal;
                    var y = state.Position.Y + dy * diagonal;

                    if (boundsDetector.IsOutOfBounds(x, y))
                    {
                        return true;
                    }

                    var tileX = (int)(x / track.TileSize);
                    var tileY = (int)(y / track.TileSize);

                    if (!track.OccupancyGrid[tileX, tileY])
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

}
