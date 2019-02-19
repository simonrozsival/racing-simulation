using System;
using static System.Math;

namespace Racing.Mathematics
{
    public readonly struct Angle : IEquatable<Angle>
    {
        public double Radians { get; }

        public static Angle Zero { get; } = new Angle(0);

        private Angle(double radians)
        {
            Radians = radians;
        }

        public override bool Equals(object obj)
            => (obj is Angle other) && Equals(other);

        public override int GetHashCode()
            => Radians.GetHashCode();

        public override string ToString()
            => $"{Radians}rad == {Radians / Math.PI * 180}°";

        public bool Equals(Angle other)
            => Radians.Equals(other.Radians);

        public Angle Clamp(double min, double max)
            => new Angle(Min(max, Max(min, Radians)));

        public static Angle operator +(Angle a, Angle b)
            => new Angle(a.Radians + b.Radians);

        public static Angle operator -(Angle a)
            => new Angle(-a.Radians);

        public static Angle operator -(Angle a, Angle b)
            => new Angle(a.Radians - b.Radians);

        public static Angle operator *(double scale, Angle a)
            => new Angle(scale * a.Radians);

        public static Angle operator *(Angle a, double scale)
            => new Angle(scale * a.Radians);

        public static Angle operator /(Angle a, double divisor)
            => new Angle(a.Radians / divisor);

        public static bool operator ==(Angle a, Angle b)
            => a.Radians == b.Radians;

        public static bool operator !=(Angle a, Angle b)
            => a.Radians != b.Radians;

        public static implicit operator Angle(double radians)
            => new Angle(radians);

        public static Angle FromDegrees(double degrees)
            => new Angle(degrees / 180.0 * PI);
    }
}
