using Racing.Model.Vehicle;
using System;

namespace Racing.Model.Visualization
{
    public sealed class ScoredPath : IVisualization
    {
        public ScoredPath(VehicleState[] states, double score, TimeSpan? duration)
        {
            States = states;
            Score = score;
            Duration = duration?.TotalSeconds;
        }

        public double? Duration { get; }
        public VehicleState[] States { get; }
        public double Score { get; }
    }
}
