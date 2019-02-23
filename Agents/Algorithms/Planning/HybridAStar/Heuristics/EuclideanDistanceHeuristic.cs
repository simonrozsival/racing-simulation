
using Racing.Mathematics;
using Racing.Model;
using System;

namespace Racing.Agents.Algorithms.Planning.HybridAStar.Heuristics
{
    internal class EuclideanDistanceHeuristic : IHeuristic
    {
        private readonly Velocity maxVelocity;
        private readonly Vector goal;

        public EuclideanDistanceHeuristic(Velocity maxVelocity, IGoal goal)
        {
            this.maxVelocity = maxVelocity;
            this.goal = goal.Position;
        }

        public TimeSpan EstimateTimeToGoal(IState state)
        {
            var distance = (state.Position - goal).CalculateLength();
            return distance / maxVelocity;
        }
    }
}
