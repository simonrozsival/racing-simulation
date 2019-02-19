using Racing.Model;
using System;

namespace Racing.Agents.Algorithms.Planning.HybridAStar.Heuristics
{
    internal sealed class DijkstraAkaNoHeuristic : IHeuristic
    {
        public TimeSpan EstimateTimeToGoal(IState state, IGoal goal)
        {
            return TimeSpan.Zero;
        }
    }
}
