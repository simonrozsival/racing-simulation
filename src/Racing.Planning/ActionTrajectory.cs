using Racing.Model;
using Racing.Model.Planning;
using Racing.Model.Vehicle;
using System;

namespace Racing.Planning
{
    internal sealed class ActionTrajectory : IActionTrajectory
    {
        public ActionTrajectory(
            TimeSpan time,
            VehicleState state,
            IAction? action,
            int targetWayPoint)
        {
            Time = time;
            State = state;
            Action = action;
            TargetWayPoint = targetWayPoint;
        }

        public TimeSpan Time { get; }
        public VehicleState State { get; }
        public IAction? Action { get; }
        public int TargetWayPoint { get; }
    }
}
