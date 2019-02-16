using Racing.Mathematics;
using Racing.Model.VehicleModel;

namespace RacingModel
{
    internal class Goal
    {
        private readonly double minimumDistanceForReachingSquared;

        public Point Position { get; }

        public Goal(Point position, double minimumDistanceForReaching)
        {
            Position = position;

            minimumDistanceForReachingSquared = minimumDistanceForReaching * minimumDistanceForReaching;
        }

        public bool ReachedGoal(VehicleState state)
            => Position.DistanceSq(state.Position) <= minimumDistanceForReachingSquared;
    }
}
