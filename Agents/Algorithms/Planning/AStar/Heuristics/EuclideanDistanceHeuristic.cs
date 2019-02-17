
using Racing.Model;
using System;

namespace Racing.Agents.Algorithms.Planning.AStar.Heuristics
{
    internal class EuclideanDistanceHeuristic : IHeuristic
    {
        public TimeSpan EstimateTimeToGoal(IState state, PlanningProblem problem)
        {
            var distance = distanceToGoal(state, problem);
            return TimeSpan.FromSeconds(distance / problem.VehicleModel.MaxVelocity);
        }

        private double distanceToGoal(IState state, PlanningProblem problem)
            => (state.Position - problem.Goal.Position).CalculateLength();
    }
}
