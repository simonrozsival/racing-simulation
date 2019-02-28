using System;

namespace Racing.Model.Planning
{
    public interface IActionTrajectory
    {
        TimeSpan Time { get; }
        IState State { get; }
        IAction? Action { get; }
        int TargetWayPoint { get; }
    }
}
