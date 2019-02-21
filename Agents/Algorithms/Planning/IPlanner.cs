using Racing.Model;
using System;

namespace Racing.Agents.Algorithms.Planning
{
    internal interface IPlanner
    {
        IPlan? FindOptimalPlanFor(IState initialState);
        IObservable<IState> ExploredStates { get; }
    }
}
