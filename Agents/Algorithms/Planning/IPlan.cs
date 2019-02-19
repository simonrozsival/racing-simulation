using Racing.Model;
using System;
using System.Collections.Generic;

namespace Racing.Agents.Algorithms.Planning
{
    public interface IPlan
    {
        TimeSpan TimeToGoal { get; }
        IList<IState> States { get; }
        IList<IAction> Actions { get; }
    }
}
