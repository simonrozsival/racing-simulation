namespace Racing.Model.CollisionDetection
{
    internal sealed class NoCollisionDetection : ICollisionDetector
    {
        public double DistanceToClosestObstacle(IState state)
        {
            return double.MaxValue;
        }

        public bool IsCollision(IState state)
        {
            return false;
        }
    }
}
