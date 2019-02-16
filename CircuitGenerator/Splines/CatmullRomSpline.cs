﻿using System;
using System.Collections.Generic;
using static RaceCircuitGenerator.Splines.BezierCurve;

namespace RaceCircuitGenerator.Splines
{
    internal sealed class CatmullRomSpline
    {
        public IReadOnlyList<Point> Points { get; }

        public CatmullRomSpline(IList<Point> points)
        {
            Points = new List<Point>(points).AsReadOnly();
        }

        public BezierCurve ToBezierCurve()
        {
            var segments = new List<Segment>(Points.Count);

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

        private static Segment createBezierSegment(Point a, Point b, Point c, Point d)
            => new Segment(
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
