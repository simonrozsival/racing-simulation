using System;
using static System.Math;

namespace Racing.Mathematics
{
    public readonly struct Vector : IEquatable<Vector>
    {
        public double X { get; }
        public double Y { get; }

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Vector CalculateNormal()
        {
            var normalized = Normalize();
            return new Vector(-normalized.Y, normalized.X);
        }

        public Vector Normalize()
        {
            var length = CalculateLength();
            return (1 / length) * this;
        }

        public double CalculateLength()
            => Sqrt(X * X + Y * Y);

        public Vector Rotate(double angle)
            => new Vector(
                x: X * Cos(angle) - Y * Sin(angle),
                y: X * Sin(angle) + Y * Cos(angle));

        public Vector Rotate(Vector center, double angle)
            => (this - center).Rotate(angle) + center;

        public double DistanceSq(Vector other)
        {
            var dx = X - other.X;
            var dy = Y - other.Y;
            return dx * dx + dy * dy;
        }

        public double Cross(Vector other)
            => X * other.Y - Y * other.X;

        public double Dot(Vector other)
            => X * other.X + Y * other.Y;

        public double Direction()
            => Atan(Y / X);

        public static Vector operator +(Vector a, Vector b)
            => new Vector(a.X + b.X, a.Y + b.Y);

        public static Vector operator -(Vector a, Vector b)
            => new Vector(a.X - b.X, a.Y - b.Y);

        public static Vector operator *(double scale, Vector a)
            => new Vector(scale * a.X, scale * a.Y);

        public override string ToString()
            => FormattableString.Invariant($"[{X}, {Y}]");

        public static bool operator ==(Vector a, Vector b)
            => a.Equals(b);

        public static bool operator !=(Vector a, Vector b)
            => !(a == b);

        public override bool Equals(object obj)
            => (obj is Vector other) && Equals(other);

        public bool Equals(Vector other)
            => (X, Y) == (other.X, other.Y);

        public override int GetHashCode()
            => HashCode.Combine(X, Y);

        public static Vector From(double d, double a)
            => new Vector(
                x: d * Cos(a),
                y: d * Sin(a));
    }
}
