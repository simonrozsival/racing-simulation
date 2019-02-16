using System.Collections;
using System.Collections.Generic;

namespace RacePlanning.Model.Math
{
    internal sealed class Rectangle : IEnumerable<Point>
    {
        public Point A { get; }
        public Point B { get; }
        public Point C { get; }
        public Point D { get; }

        public Rectangle(Point a, Point b, Point c, Point d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }

        public Rectangle Rotate(Point center, double angle)
            => new Rectangle(
                A.Rotate(center, angle),
                B.Rotate(center, angle),
                C.Rotate(center, angle),
                D.Rotate(center, angle));

        public IEnumerator<Point> GetEnumerator()
        {
            yield return A;
            yield return B;
            yield return C;
            yield return D;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
