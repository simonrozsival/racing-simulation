using Racing.Mathematics;

namespace Racing.Model.Visualization
{
    public sealed class Path : IVisualization
    {
        public Path(double? duration, Vector[] points)
        {
            Duration = duration;
            Points = points;
        }

        public double? Duration { get; }
        public Vector[] Points { get; }
    }
}
