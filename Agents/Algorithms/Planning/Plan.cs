using System;
using System.Collections.Generic;
using Racing.Model;

namespace Racing.Agents.Algorithms.Planning
{
    internal sealed class Plan : IPlan
    {
        public Plan(TimeSpan timeToGoal, IList<IState> states, IList<IAction> actions)
        {
            TimeToGoal = timeToGoal;
            States = states;
            Actions = actions;
        }

        public TimeSpan TimeToGoal { get; }
        public IList<IState> States { get; }
        public IList<IAction> Actions { get; }
    }
}
