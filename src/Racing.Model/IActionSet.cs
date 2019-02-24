using System.Collections.Generic;

namespace Racing.Model
{
    public interface IActionSet
    {
        IAction FullThrustForward { get; }
        IAction Brake { get; }
        IReadOnlyList<IAction> AllPossibleActions { get; }
    }
}
