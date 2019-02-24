using Racing.Model;
using System;

namespace Racing.Planning.Algorithms.HybridAStar.Heuristics
{
    interface IHeuristic
    {
        TimeSpan EstimateTimeToGoal(IState state, int nextWayPoint);
    }
}
