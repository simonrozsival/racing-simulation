using Racing.Model;
using Racing.Model.Vehicle;
using System;

namespace Racing.Planning.Algorithms.HybridAStar.Heuristics
{
    interface IHeuristic
    {
        TimeSpan EstimateTimeToGoal(VehicleState state, int nextWayPoint);
    }
}
