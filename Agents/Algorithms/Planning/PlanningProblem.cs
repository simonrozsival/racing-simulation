using Racing.Model;
using System.Collections.Immutable;

namespace Racing.Agents.Algorithms.Planning
{
    internal sealed class PlanningProblem
    {
        public IState InitialState { get; }
        public IImmutableList<IAction> PossibleActions { get; }
        public IGoal Goal { get; }

        public PlanningProblem(
            IState initialState,
            IImmutableList<IAction> actions,
            IGoal goal)
        {
            InitialState = initialState;
            PossibleActions = actions;
            Goal = goal;
        }
    }
}
