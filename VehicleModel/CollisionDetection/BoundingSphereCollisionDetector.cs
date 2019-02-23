using Racing.Mathematics;
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
            => IsCollision(state.Position);

        public bool IsCollision(Vector position)
        {
            for (var dx = -1; dx <= 1; dx++)
            {
                for (var dy = -1; dy <= 1; dy++)
                {
                    var x = position.X + dx * diagonal;
                    var y = position.Y + dy * diagonal;

                    if (collides(x, y))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool collides(double x, double y)
        {
            if (boundsDetector.IsOutOfBounds(x, y))
            {
                return true;
            }

            return track.IsOccupied(x, y);
        }
    }

}
