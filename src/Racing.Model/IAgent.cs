using Racing.Model.Vehicle;
using Racing.Model.Visualization;
using System;

namespace Racing.Model
{
    public interface IAgent
    {
        IObservable<IVisualization> Visualization { get; }
        IAction ReactTo(VehicleState state, int waypoint);
    }
}
