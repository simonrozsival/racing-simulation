using Racing.Model;
using Racing.Model.Planning;
using System;

namespace Racing.Planning
{
    internal sealed class ActionTrajectory : IActionTrajectory
    {
        public ActionTrajectory(
            TimeSpan time,
            IState state,
            IAction? action,
            int targetWayPoint)
        {
            Time = time;
            State = state;
            Action = action;
            TargetWayPoint = targetWayPoint;
        }

        public TimeSpan Time { get; }
        public IState State { get; }
        public IAction? Action { get; }
        public int TargetWayPoint { get; }
    }
}
