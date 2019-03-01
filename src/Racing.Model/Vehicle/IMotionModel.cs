using System;
using System.Collections.Generic;

namespace Racing.Model.Vehicle
{
    public interface IMotionModel
    {
        IEnumerable<(TimeSpan relativeTime, VehicleState state)> CalculateNextState(VehicleState state, IAction action, TimeSpan simulationTime);
    }
}