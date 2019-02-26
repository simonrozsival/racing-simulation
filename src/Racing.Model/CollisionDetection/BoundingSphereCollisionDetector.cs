using Racing.Mathematics;
using Racing.Model.Vehicle;
using System;

namespace Racing.Model.CollisionDetection
{
    public sealed class BoundingSphereCollisionDetector : ICollisionDetector
    {
        private readonly ITrack track;
        private readonly double diagonal;


        public BoundingSphereCollisionDetector(ITrack track, IVehicleModel vehicleModel)
        {
            this.track = track;

            var u = vehicleModel.Length / 2;
            var v = vehicleModel.Width / 2;
            diagonal = Math.Sqrt(u * u + v * v);
        }

        public Length DistanceToClosestObstacle(IState state)
            => track.DistanceToClosestObstacle(state.Position) - diagonal;

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

                    if (track.IsOccupied(x, y))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

}
