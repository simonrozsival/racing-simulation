using System;
using System.Collections.Generic;

namespace Racing.Model.Planning
{
    public interface IPlan
    {
        TimeSpan TimeToGoal { get; }
        IReadOnlyList<IActionTrajectory> Trajectory { get; }
        IPlan ToDetailedPlan(IWorldDefinition world);
    }
}
