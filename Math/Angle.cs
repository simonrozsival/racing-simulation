using static System.Math;

namespace Racing.Mathematics
{
    public struct Angle
    {
        public double Radians { get; }

        public static Angle Zero { get; } = new Angle(0);

        private Angle(double radians)
        {
            Radians = clamp(radians);
        }

        public Angle Clamp(Angle min, Angle max)
            => new Angle(Min(max.Radians, Max(min.Radians, Radians)));

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

        public static implicit operator Angle(double radians)
            => new Angle(radians);

        public static Angle FromDegrees(double degrees)
            => new Angle(degrees / 180.0 * PI);

        private static double clamp(double angle)
        {
            while (angle > 2 * PI) angle -= 2 * PI;
            while (angle < 0) angle += 2 * PI;
            return angle;
        }
    }
}
