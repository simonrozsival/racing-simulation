using System;

namespace Racing.Mathematics
{
    public readonly struct Distance : IEquatable<Distance>
    {
        public double Meters { get; }

        private Distance(double meters)
        {
            if (meters < 0)
            {
                throw new ArgumentOutOfRangeException($"Distances can't be negative.");
            }

            Meters = meters;
        }

        public override int GetHashCode()
            => Meters.GetHashCode();

        public override bool Equals(object obj)
            => (obj is Distance other) && Equals(other);

        public bool Equals(Distance distance)
            => Meters == distance.Meters;

        public static Distance operator +(Distance a, Distance b)
            => new Distance(a.Meters + b.Meters);

        public static Distance operator -(Distance a, Distance b)
            => new Distance(a.Meters - b.Meters);

        public static Distance operator *(double a, Distance b)
            => new Distance(a * b.Meters);

        public static Distance operator /(Distance a, Distance b)
            => new Distance(a.Meters / b.Meters);

        public static Distance operator /(double a, Distance b)
            => new Distance(a / b.Meters);

        public static bool operator ==(Distance a, Distance b)
            => a.Meters == b.Meters;

        public static bool operator !=(Distance a, Distance b)
            => a.Meters != b.Meters;

        public static bool operator <=(Distance a, Distance b)
            => a.Meters <= b.Meters;

        public static bool operator >=(Distance a, Distance b)
            => a.Meters >= b.Meters;

        public static bool operator <(Distance a, Distance b)
            => a.Meters < b.Meters;

        public static bool operator >(Distance a, Distance b)
            => a.Meters < b.Meters;

        public static implicit operator Distance(double meters)
            => new Distance(meters);

        public static Distance FromMeters(double meters)
            => new Distance(meters);
        public static Distance Between(Vector a, Vector b)
            => new Distance((a - b).CalculateLength());
    }
}
