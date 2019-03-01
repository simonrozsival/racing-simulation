using Racing.Model.Vehicle;

namespace Racing.Model.CollisionDetection
{
    internal sealed class NoCollisionDetection : ICollisionDetector
    {
        public double DistanceToClosestObstacle(VehicleState state)
        {
            return double.MaxValue;
        }

        public bool IsCollision(VehicleState state)
        {
            return false;
        }
    }
}
