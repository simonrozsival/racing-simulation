using System.Collections.Generic;

namespace Racing.Mathematics.Splines
{
    public sealed class CatmullRomSpline
    {
        public IReadOnlyList<Point> Points { get; }

        public CatmullRomSpline(IEnumerable<Point> points)
        {
            Points = new List<Point>(points).AsReadOnly();
        }

        public BezierCurve ToBezierCurve()
        {
            var segments = new List<BezierCurve.Segment>(Points.Count);

            for (int i = 0; i < Points.Count; i++)
            {
                var a = Points[(i - 1 + Points.Count) % Points.Count];
                var b = Points[i];
                var c = Points[(i + 1) % Points.Count];
                var d = Points[(i + 2) % Points.Count];

                segments.Add(createBezierSegment(a, b, c, d));
            }

            return new BezierCurve(segments);
        }

        private static BezierCurve.Segment createBezierSegment(Point a, Point b, Point c, Point d)
            => new BezierCurve.Segment(
                start: b,
                startControlPoint:
                    new Point(
                        x: (-1.0 / 6.0) * a.X + b.X + (1.0 / 6.0) * c.X,
                        y: (-1.0 / 6.0) * a.Y + b.Y + (1.0 / 6.0) * c.Y),
                end: c,
                endControlPoint:
                    new Point(
                        x: (1.0 / 6.0) * b.X + c.X + (-1.0 / 6.0) * d.X,
                        y: (1.0 / 6.0) * b.Y + c.Y + (-1.0 / 6.0) * d.Y));
    }
}
