using System;
using Racing.Model;
using Racing.Model.Vehicle;

namespace Racing.Agents
{
    public sealed class RandomAgent : IAgent
    {
        private readonly Random random;

        public RandomAgent(Random random)
        {
            this.random = random;
        }

        public IAction ReactTo(IState state)
            => SteeringInput.PossibleActions[random.Next(0, SteeringInput.PossibleActions.Count - 1)];
    }
}
