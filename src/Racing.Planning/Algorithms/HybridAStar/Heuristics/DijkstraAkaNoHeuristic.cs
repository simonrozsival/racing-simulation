using Racing.Model;
using Racing.Model.Vehicle;
using System;

namespace Racing.Planning.Algorithms.HybridAStar.Heuristics
{
    internal sealed class DijkstraAkaNoHeuristic : IHeuristic
    {
        public TimeSpan EstimateTimeToGoal(VehicleState state, int nextWayPoint)
        {
            return TimeSpan.Zero;
        }
    }
}
