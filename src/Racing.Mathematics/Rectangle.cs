using System.Collections;
using System.Collections.Generic;

namespace Racing.Mathematics
{
    public struct Rectangle : IEnumerable<Vector>
    {
        public Vector A { get; }
        public Vector B { get; }
        public Vector C { get; }
        public Vector D { get; }

        public Rectangle(Vector a, Vector b, Vector c, Vector d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }

        public Rectangle Rotate(Vector center, double angle)
            => new Rectangle(
                A.Rotate(center, angle),
                B.Rotate(center, angle),
                C.Rotate(center, angle),
                D.Rotate(center, angle));

        public IEnumerator<Vector> GetEnumerator()
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
