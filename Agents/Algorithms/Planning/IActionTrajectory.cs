using Racing.Model;
using System;

namespace Racing.Agents.Algorithms.Planning
{
    public interface IActionTrajectory
    {
        TimeSpan Time { get; }
        IState State { get; }
        IAction? Action { get; }
    }
}
