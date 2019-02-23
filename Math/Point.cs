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

        public Vector Rotate(Angle angle)
            => new Vector(
                x: X * Cos(angle.Radians) - Y * Sin(angle.Radians),
                y: X * Sin(angle.Radians) + Y * Cos(angle.Radians));

        public Vector Rotate(Vector center, Angle angle)
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

        public Angle Direction()
            => Atan(Y / X);

        public static Vector operator +(Vector a, Vector b)
            => new Vector(a.X + b.X, a.Y + b.Y);

        public static Vector operator -(Vector a, Vector b)
            => new Vector(a.X - b.X, a.Y - b.Y);

        public static Vector operator *(double scale, Vector a)
            => new Vector(scale * a.X, scale * a.Y);

        public override string ToString()
            => FormattableString.Invariant($"Point[{X}, {Y}]");

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
    }
}
