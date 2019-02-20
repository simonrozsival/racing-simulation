using Racing.Model;

namespace Racing.Agents.Algorithms.Planning
{
    internal sealed class PlanningProblem
    {
        public IState InitialState { get; }
        public IActionSet Actions { get; }
        public IGoal Goal { get; }

        public PlanningProblem(
            IState initialState,
            IActionSet actions,
            IGoal goal)
        {
            InitialState = initialState;
            Actions = actions;
            Goal = goal;
        }
    }
}
