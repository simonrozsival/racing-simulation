using System;
using System.Collections.Generic;

namespace Racing.Model.Vehicle
{
    public interface IMotionModel
    {
        IEnumerable<(TimeSpan relativeTime, IState state)> CalculateNextState(IState state, IAction action, TimeSpan simulationTime);
    }
}