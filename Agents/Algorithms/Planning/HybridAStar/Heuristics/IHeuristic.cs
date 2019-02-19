using Racing.Model;
using System;

namespace Racing.Agents.Algorithms.Planning.HybridAStar.Heuristics
{
    interface IHeuristic
    {
        TimeSpan EstimateTimeToGoal(IState state, IGoal goal);
    }
}
