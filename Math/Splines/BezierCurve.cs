using System.Collections.Generic;

namespace Racing.Mathematics.Splines
{
    public sealed class BezierCurve
    {
        public IList<Segment> Segments { get; }

        public BezierCurve(IList<Segment> segments)
        {
            Segments = segments;
        }

        public sealed class Segment
        {
            public Vector Start { get; }
            public Vector End { get; }
            public Vector StartControlPoint { get; }
            public Vector EndControlPoint { get; }

            public Segment(
                Vector start,
                Vector startControlPoint,
                Vector end,
                Vector endControlPoint)
            {
                Start = start;
                StartControlPoint = startControlPoint;
                End = end;
                EndControlPoint = endControlPoint;
            }
        }
    }
}
