using Racing.Model;
using Racing.Model.Planning;
using System;

namespace Racing.Planning
{
    internal interface IPlanner
    {
        IPlan? FindOptimalPlanFor(IState initialState);
        IObservable<IState> ExploredStates { get; }
    }
}
