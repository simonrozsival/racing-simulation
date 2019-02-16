using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Racing.Model;

namespace Racing.Agents
{
    public sealed class RandomAgent : IAgent
    {
        public IObservable<IAction> Actions { get; }

        public IObserver<IState> Perception { get; }

        public RandomAgent(Random random)
        {
            IAction randomAction()
                => SteeringAction.PossibleActions[random.Next(0, SteeringAction.PossibleActions.Count - 1)];

            var perceptionSubject = new Subject<IState>();
            Perception = perceptionSubject;
            Actions = perceptionSubject.Select(_ =>
            {
                Console.WriteLine("random action");
                return randomAction();
            });
        }
    }
}
