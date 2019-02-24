using Racing.Model;
using System;

namespace Racing.Planning.Algorithms.HybridAStar.Heuristics
{
    internal sealed class DijkstraAkaNoHeuristic : IHeuristic
    {
        public TimeSpan EstimateTimeToGoal(IState state, int nextWayPoint)
        {
            return TimeSpan.Zero;
        }
    }
}
