using Racing.Mathematics;

namespace Racing.Model
{
    public class RadialGoal : IGoal
    {
        private readonly Length minimumDistanceForReachingSquared;

        public Vector Position { get; }

        public RadialGoal(Vector position, double minimumDistanceForReaching)
        {
            Position = position;

            minimumDistanceForReachingSquared = minimumDistanceForReaching * minimumDistanceForReaching;
        }

        public bool ReachedGoal(Vector position)
            => Position.DistanceSq(position) <= minimumDistanceForReachingSquared;
    }
}
