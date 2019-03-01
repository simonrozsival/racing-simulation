using Racing.Model;
using Racing.Model.Planning;
using Racing.Model.Vehicle;
using System;

namespace Racing.Planning
{
    internal interface IPlanner
    {
        IPlan? FindOptimalPlanFor(VehicleState initialState);
        IObservable<VehicleState> ExploredStates { get; }
    }
}
