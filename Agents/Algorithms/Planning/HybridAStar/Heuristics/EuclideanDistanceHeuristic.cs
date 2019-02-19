
using Racing.Model;
using System;

namespace Racing.Agents.Algorithms.Planning.HybridAStar.Heuristics
{
    internal class EuclideanDistanceHeuristic : IHeuristic
    {
        private readonly double maxVelocity;

        public EuclideanDistanceHeuristic(double maxVelocity)
        {
            this.maxVelocity = maxVelocity;
        }

        public TimeSpan EstimateTimeToGoal(IState state, IGoal goal)
        {
            var distance = distanceToGoal(state, goal);
            return TimeSpan.FromSeconds(distance / maxVelocity);
        }

        private double distanceToGoal(IState state, IGoal goal)
            => (state.Position - goal.Position).CalculateLength();
    }
}
