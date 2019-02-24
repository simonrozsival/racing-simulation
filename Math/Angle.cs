using System;
using static System.Math;

namespace Racing.Mathematics
{
    public readonly struct Angle : IEquatable<Angle>
    {
        public double Radians { get; }

        public static Angle Zero { get; } = Angle.FromDegrees(0);
        public static Angle FullCircle { get; } = Angle.FromDegrees(360);

        private Angle(double radians)
        {
            Radians = radians;
        }

        public override bool Equals(object obj)
            => (obj is Angle other) && Equals(other);

        public override int GetHashCode()
            => Radians.GetHashCode();

        public override string ToString()
            => $"{Radians}rad == {Radians / PI * 180}°";

        public bool Equals(Angle other)
            => Radians.Equals(other.Radians);

        public Angle Clamp(double min, double max)
            => Min(max, Max(min, Radians));

        public static Angle operator +(Angle a, Angle b)
            => a.Radians + b.Radians;

        public static Angle operator -(Angle a)
            => -a.Radians;

        public static Angle operator -(Angle a, Angle b)
            => a.Radians - b.Radians;

        public static Angle operator *(double scale, Angle a)
            => scale * a.Radians;
        public static Angle operator *(Length scale, Angle a)
            => scale.Meters * a.Radians;

        public static Angle operator *(Angle a, double scale)
            => scale * a.Radians;

        public static Angle operator /(Angle a, Angle b)
            => a.Radians / b.Radians;

        public static Angle operator /(Angle a, double divisor)
            => a.Radians / divisor;

        public static bool operator ==(Angle a, Angle b)
            => a.Radians == b.Radians;

        public static bool operator !=(Angle a, Angle b)
            => a.Radians != b.Radians;

        public static bool operator <=(Angle a, Angle b)
            => a.Radians <= b.Radians;

        public static bool operator >=(Angle a, Angle b)
            => a.Radians >= b.Radians;

        public static bool operator <(Angle a, Angle b)
            => a.Radians < b.Radians;

        public static bool operator >(Angle a, Angle b)
            => a.Radians > b.Radians;

        public static implicit operator Angle(double radians)
            => new Angle(radians);

        public static explicit operator int(Angle angle)
            => (int)angle.Radians;

        public static Angle FromDegrees(double degrees)
            => new Angle(degrees / 180.0 * PI);
    }
}
