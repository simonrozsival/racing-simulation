using RacePlanning.Model.Math;
using RacePlanning.Model.VehicleModel;

namespace RacePlanning.Model
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
        {
            var dx = state.Position.X - Position.X;
            var dy = state.Position.Y - Position.Y;
            var distanceSq = dx * dx + dy * dy;
            return distanceSq <= minimumDistanceForReachingSquared;
        }
    }
}
