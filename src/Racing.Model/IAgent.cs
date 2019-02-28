using Racing.Model.Visualization;
using System;

namespace Racing.Model
{
    public interface IAgent
    {
        IObservable<IVisualization> Visualization { get; }
        IAction ReactTo(IState state, int waypoint);
    }
}
