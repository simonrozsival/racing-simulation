using Racing.Model;
using System.Collections.Generic;

namespace Racing.Agents.Algorithms.Planning
{
    internal interface IPlanner
    {
        IEnumerable<IAction> FindOptimalPlanFor(PlanningProblem problem);
    }
}
