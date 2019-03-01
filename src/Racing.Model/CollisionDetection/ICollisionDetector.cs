using Racing.Model.Vehicle;

namespace Racing.Model.CollisionDetection
{
    public interface ICollisionDetector
    {
        bool IsCollision(VehicleState state);
        double DistanceToClosestObstacle(VehicleState state);
    }
}
