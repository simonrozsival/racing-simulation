using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Racing.Model;

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
            => SteeringAction.PossibleActions[random.Next(0, SteeringAction.PossibleActions.Count - 1)];
    }
}
