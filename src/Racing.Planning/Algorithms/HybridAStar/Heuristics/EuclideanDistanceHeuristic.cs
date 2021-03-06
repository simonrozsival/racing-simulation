﻿
using Racing.Mathematics;
using Racing.Model;
using Racing.Model.Vehicle;
using System;

namespace Racing.Planning.Algorithms.HybridAStar.Heuristics
{
    internal class EuclideanDistanceHeuristic : IHeuristic
    {
        private readonly double maxVelocity;
        private readonly Vector goal;

        public EuclideanDistanceHeuristic(double maxVelocity, IGoal goal)
        {
            this.maxVelocity = maxVelocity;
            this.goal = goal.Position;
        }

        public TimeSpan EstimateTimeToGoal(VehicleState state, int nextWayPoint)
        {
            var distance = (state.Position - goal).CalculateLength();
            return TimeSpan.FromSeconds(distance / maxVelocity);
        }
    }
}
