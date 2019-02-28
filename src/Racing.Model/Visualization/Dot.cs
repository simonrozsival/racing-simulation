using System;
using Racing.Mathematics;

namespace Racing.Model.Visualization
{
    public sealed class Dot : IVisualization
    {
        public Dot(Vector center, double radius, string color, TimeSpan? duration)
        {
            Center = center;
            Radius = radius;
            Color = color;
            Duration = duration?.TotalSeconds;
        }

        public double? Duration { get; }
        public Vector Center { get; }
        public double Radius { get; }
        public string Color { get; }
    }
}
