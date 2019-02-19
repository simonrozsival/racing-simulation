
using Racing.Mathematics;
using Racing.Model;
using System;

namespace Racing.Agents.Algorithms.Planning.HybridAStar.Heuristics
{
    internal class EuclideanDistanceHeuristic : IHeuristic
    {
        private readonly double maxVelocity;
        private readonly Point goal;

        public EuclideanDistanceHeuristic(double maxVelocity, IGoal goal)
        {
            this.maxVelocity = maxVelocity;
            this.goal = goal.Position;
        }

        public TimeSpan EstimateTimeToGoal(IState state)
        {
            var distance = distanceToGoal(state);
            return TimeSpan.FromSeconds(distance / maxVelocity);
        }

        private double distanceToGoal(IState state)
            => (state.Position - goal).CalculateLength();
    }
}
