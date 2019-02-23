using System;

namespace Racing.Mathematics
{
    public readonly struct Length : IEquatable<Length>
    {
        public double Meters { get; }

        public static Length Zero { get; } = FromMeters(0);

        private Length(double meters)
        {
            Meters = meters;
        }

        public Length Squared()
            => Meters * Meters;

        public override int GetHashCode()
            => Meters.GetHashCode();

        public override bool Equals(object obj)
            => (obj is Length other) && Equals(other);

        public bool Equals(Length distance)
            => Meters == distance.Meters;

        public static Length operator -(Length a)
            => -a.Meters;

        public static Length operator +(Length a, Length b)
            => a.Meters + b.Meters;

        public static Length operator -(Length a, Length b)
            => a.Meters - b.Meters;

        public static Length operator *(Length a, Length b)
            => a.Meters * b.Meters;

        public static Length operator *(double a, Length b)
            => a * b.Meters;

        public static Length operator /(Length a, Length b)
            => a.Meters / b.Meters;

        public static Length operator /(double a, Length b)
            => a / b.Meters;

        public static bool operator ==(Length a, Length b)
            => a.Meters == b.Meters;

        public static bool operator !=(Length a, Length b)
            => a.Meters != b.Meters;

        public static bool operator <=(Length a, Length b)
            => a.Meters <= b.Meters;

        public static bool operator >=(Length a, Length b)
            => a.Meters >= b.Meters;

        public static bool operator <(Length a, Length b)
            => a.Meters < b.Meters;

        public static bool operator >(Length a, Length b)
            => a.Meters < b.Meters;

        public static implicit operator Length(double meters)
            => new Length(meters);

        public static Length FromMeters(double meters)
            => new Length(meters);

        public static Length Between(Vector a, Vector b)
            => (a - b).CalculateLength();
    }
}
