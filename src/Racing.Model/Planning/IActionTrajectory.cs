using Racing.Model.Vehicle;
using System;

namespace Racing.Model.Planning
{
    public interface IActionTrajectory
    {
        TimeSpan Time { get; }
        VehicleState State { get; }
        IAction? Action { get; }
        int TargetWayPoint { get; }
    }
}
