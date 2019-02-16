using System;
using Racing.Model;

namespace Racing.Agents
{
    public interface IAgent
    {
        IObservable<IAction> Actions { get; }
        IObserver<IState> Perception { get; }
    }
}
