using System;
using Racing.Model.Vehicle;

namespace Racing.Agents
{
    public interface IAgent
    {
        IObservable<IAction> Actions { get; }
        IObserver<IState> Perception { get; }
    }
}
