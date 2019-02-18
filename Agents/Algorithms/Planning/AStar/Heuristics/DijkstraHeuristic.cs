using Racing.Model;
using System;

namespace Racing.Agents.Algorithms.Planning.AStar.Heuristics
{
    internal sealed class DijkstraAkaNoHeuristic : IHeuristic
    {
        public TimeSpan EstimateTimeToGoal(IState state, PlanningProblem problem)
        {
            return TimeSpan.Zero;
        }
    }
}
