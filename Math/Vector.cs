using System;
using static Racing.Mathematics.CustomMath;

namespace Racing.Mathematics
{
    public readonly struct Vector : IEquatable<Vector>
    {
        public Length X { get; }
        public Length Y { get; }

        public Vector(Length x, Length y)
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

        public Length CalculateLength()
            => Sqrt(X.Squared() + Y.Squared());

        public Vector Rotate(Angle angle)
            => new Vector(
                x: X * Cos(angle) - Y * Sin(angle),
                y: X * Sin(angle) + Y * Cos(angle));

        public Vector Rotate(Vector center, Angle angle)
            => (this - center).Rotate(angle) + center;

        public Length DistanceSq(Vector other)
        {
            var dx = X - other.X;
            var dy = Y - other.Y;
            return dx * dx + dy * dy;
        }

        public Length Cross(Vector other)
            => X * other.Y - Y * other.X;

        public Length Dot(Vector other)
            => X * other.X + Y * other.Y;

        public Angle Direction()
            => Atan(Y / X);

        public static Vector operator +(Vector a, Vector b)
            => new Vector(a.X + b.X, a.Y + b.Y);

        public static Vector operator -(Vector a, Vector b)
            => new Vector(a.X - b.X, a.Y - b.Y);

        public static Vector operator *(double scale, Vector a)
            => new Vector(scale * a.X, scale * a.Y);

        public static Vector operator *(Length scale, Vector a)
            => new Vector(scale.Meters * a.X, scale.Meters * a.Y);

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

        public static Vector From(Length d, Angle a)
            => new Vector(
                x: d.Meters * Cos(a.Radians),
                y: d.Meters * Sin(a.Radians));
    }
}
