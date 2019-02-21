using System;
using System.Collections.Generic;

namespace Racing.Agents.Algorithms.Planning
{
    internal sealed class Plan : IPlan
    {
        public Plan(TimeSpan timeToGoal, IList<IActionTrajectory> trajectory)
        {
            TimeToGoal = timeToGoal;
            Trajectory = trajectory;
        }

        public TimeSpan TimeToGoal { get; }
        public IList<IActionTrajectory> Trajectory { get; }
    }
}
