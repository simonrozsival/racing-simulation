using Racing.Mathematics;
using Racing.Model.Vehicle;
using System.Collections.Generic;

namespace Racing.Model.CollisionDetection
{
    public sealed class AccurateCollisionDetector : ICollisionDetector
    {
        private readonly ITrack track;
        private readonly IVehicleModel vehicleModel;
        private readonly BoundsDetector boundsDetector;

        public AccurateCollisionDetector(ITrack track, IVehicleModel vehicleModel)
        {
            this.track = track;
            this.vehicleModel = vehicleModel;

            boundsDetector = new BoundsDetector(track);
        }

        public bool IsCollision(IState state)
        {
            var bounds = calculateBounds(state);
            return isCollision(bounds);
        }

        private bool isCollision(IEnumerable<Point> points)
        {
            foreach (var point in points)
            {
                if (boundsDetector.IsOutOfBounds(point.X, point.Y))
                {
                    return true;
                }

                var tileX = (int)(point.X / track.TileSize);
                var tileY = (int)(point.Y / track.TileSize);

                if (!track.OccupancyGrid[tileX, tileY])
                {
                    return true;
                }
            }

            return false;
        }

        private Rectangle calculateBounds(IState state)
        {
            var ux = vehicleModel.Length / 2;
            var uy = vehicleModel.Width / 2;
            var alignedRect = new Rectangle(
                new Point(state.Position.X - ux, state.Position.Y + uy),
                new Point(state.Position.X - ux, state.Position.Y - uy),
                new Point(state.Position.X + ux, state.Position.Y - uy),
                new Point(state.Position.X + ux, state.Position.Y + uy));

            return alignedRect.Rotate(state.Position, state.HeadingAngle);
        }
    }
}
