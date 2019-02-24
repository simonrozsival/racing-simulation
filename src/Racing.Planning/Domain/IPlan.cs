using Racing.Model;
using System;
using System.Collections.Generic;

namespace Racing.Planning.Domain
{
    public interface IPlan
    {
        TimeSpan TimeToGoal { get; }
        IList<IActionTrajectory> Trajectory { get; }
    }
}
