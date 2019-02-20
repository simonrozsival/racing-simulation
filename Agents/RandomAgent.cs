using System;
using Racing.Model;
using Racing.Model.Vehicle;

namespace Racing.Agents
{
    public sealed class RandomAgent : IAgent
    {
        private readonly Random random;
        private readonly IActionSet actions;

        public RandomAgent(Random random, IActionSet actions)
        {
            this.random = random;
            this.actions = actions;
        }

        public IAction ReactTo(IState state)
            => actions.AllPossibleActions[random.Next(0, actions.AllPossibleActions.Count - 1)];
    }
}
