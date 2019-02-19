using Racing.Mathematics;
using Racing.Model.Vehicle;
using System.Collections.Generic;

namespace Racing.Model.CollisionDetection
{
    public sealed class AccurateCollisionDetector : ICollisionDetector
    {
        private readonly ITrack track;
        private readonly BoundsDetector boundsDetector;
        private readonly double ux;
        private readonly double uy;
        private readonly bool allowsBackwaradDriving = false;

        public AccurateCollisionDetector(ITrack track, IVehicleModel vehicleModel, double safetyMargin = 0)
        {
            this.track = track;

            ux = vehicleModel.Length / 2 + safetyMargin;
            uy = vehicleModel.Width / 2 + safetyMargin;

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

        private IEnumerable<Point> calculateBounds(IState state)
        {
            yield return new Point(state.Position.X - ux, state.Position.Y + uy).Rotate(state.Position, state.HeadingAngle);
            yield return new Point(state.Position.X + ux, state.Position.Y + uy).Rotate(state.Position, state.HeadingAngle);

            if (allowsBackwaradDriving)
            {
                yield return new Point(state.Position.X - ux, state.Position.Y - uy).Rotate(state.Position, state.HeadingAngle);
                yield return new Point(state.Position.X + ux, state.Position.Y - uy).Rotate(state.Position, state.HeadingAngle);
            }
        }
    }
}
