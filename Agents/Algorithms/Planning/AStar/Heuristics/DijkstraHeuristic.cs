using Racing.Model;
using System;

namespace Racing.Agents.Algorithms.Planning.AStar.Heuristics
{
    internal sealed class DijkstraHeuristic : IHeuristic
    {
        public TimeSpan EstimateTimeToGoal(IState state, PlanningProblem problem)
        {
            return TimeSpan.Zero;
        }
    }
}
