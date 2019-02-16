using Racing.Mathematics;

namespace Racing.Model
{
    public class RadialGoal : IGoal
    {
        private readonly double minimumDistanceForReachingSquared;

        public Point Position { get; }

        public RadialGoal(Point position, double minimumDistanceForReaching)
        {
            Position = position;

            minimumDistanceForReachingSquared = minimumDistanceForReaching * minimumDistanceForReaching;
        }

        public bool ReachedGoal(IState state)
            => Position.DistanceSq(state.Position) <= minimumDistanceForReachingSquared;
    }
}
