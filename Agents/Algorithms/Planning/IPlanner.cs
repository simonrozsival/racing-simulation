using Racing.Model;
using System;

namespace Racing.Agents.Algorithms.Planning
{
    internal interface IPlanner
    {
        IPlan? FindOptimalPlanFor(PlanningProblem problem);
        IObservable<IState> ExploredStates { get; }
    }
}
