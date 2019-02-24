using Racing.Model;
using System;

namespace Racing.Planning.Domain
{
    public interface IActionTrajectory
    {
        TimeSpan Time { get; }
        IState State { get; }
        IAction? Action { get; }
    }
}
