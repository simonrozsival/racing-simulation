using System;

namespace Racing.Model.Visualization
{
    public sealed class ScoredPath : IVisualization
    {
        public ScoredPath(IState[] states, double score, TimeSpan? duration)
        {
            States = states;
            Score = score;
            Duration = duration?.TotalSeconds;
        }

        public double? Duration { get; }
        public IState[] States { get; }
        public double Score { get; }
    }
}
