using System;

namespace Racing.Mathematics
{
    public readonly struct Velocity : IEquatable<Velocity>
    {
        public double MetersPerSecond { get; }

        private Velocity(double meters)
        {
            if (meters < 0)
            {
                throw new ArgumentOutOfRangeException($"Velocitys can't be negative.");
            }

            MetersPerSecond = meters;
        }

        public override int GetHashCode()
            => MetersPerSecond.GetHashCode();

        public override bool Equals(object obj)
            => (obj is Velocity other) && Equals(other);

        public bool Equals(Velocity Velocity)
            => MetersPerSecond == Velocity.MetersPerSecond;

        public static Velocity operator +(Velocity a, Velocity b)
            => new Velocity(a.MetersPerSecond + b.MetersPerSecond);

        public static Velocity operator -(Velocity a, Velocity b)
            => new Velocity(a.MetersPerSecond - b.MetersPerSecond);

        public static TimeSpan operator /(Length a, Velocity b)
            => TimeSpan.FromSeconds(a.Meters / b.MetersPerSecond);

        public static bool operator ==(Velocity a, Velocity b)
            => a.MetersPerSecond == b.MetersPerSecond;

        public static bool operator !=(Velocity a, Velocity b)
            => a.MetersPerSecond != b.MetersPerSecond;

        public static bool operator <=(Velocity a, Velocity b)
            => a.MetersPerSecond <= b.MetersPerSecond;

        public static bool operator >=(Velocity a, Velocity b)
            => a.MetersPerSecond >= b.MetersPerSecond;

        public static bool operator <(Velocity a, Velocity b)
            => a.MetersPerSecond < b.MetersPerSecond;

        public static bool operator >(Velocity a, Velocity b)
            => a.MetersPerSecond < b.MetersPerSecond;

        public static implicit operator Velocity(double metersPerSecond)
            => new Velocity(metersPerSecond);

        public static Velocity FromMetersPerSecond(double metersPerSecond)
            => new Velocity(metersPerSecond);
    }
}
