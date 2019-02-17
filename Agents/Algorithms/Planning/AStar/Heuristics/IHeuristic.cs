using Racing.Model;
using System;

namespace Racing.Agents.Algorithms.Planning.AStar.Heuristics
{
    interface IHeuristic
    {
        TimeSpan EstimateTimeToGoal(IState state, PlanningProblem problem);
    }
}
