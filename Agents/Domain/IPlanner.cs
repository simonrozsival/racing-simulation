using Racing.Model;
using System;

namespace Racing.Planning.Domain
{
    internal interface IPlanner
    {
        IPlan? FindOptimalPlanFor(IState initialState);
        IObservable<IState> ExploredStates { get; }
    }
}
