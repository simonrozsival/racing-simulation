using System.Collections.Generic;
using System.Linq;

namespace RaceCircuitGenerator.Splines
{
    internal sealed class BezierCurve
    {
        public IList<Segment> Segments { get; }

        public BezierCurve(IList<Segment> segments)
        {
            Segments = segments;
        }

        public sealed class Segment
        {
            public Point Start { get; }
            public Point End { get; }
            public Point StartControlPoint { get; }
            public Point EndControlPoint { get; }

            public Segment(
                Point start,
                Point startControlPoint,
                Point end,
                Point endControlPoint)
            {
                Start = start;
                StartControlPoint = startControlPoint;
                End = end;
                EndControlPoint = endControlPoint;
            }
        }
    }
}
