namespace Racing.Agents.Algorithms.Planning
{
    internal interface IPlanner
    {
        IPlan? FindOptimalPlanFor(PlanningProblem problem);
    }
}
