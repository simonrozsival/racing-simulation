using System;

namespace Racing.Mathematics
{
    public static class CustomMath
    {
        public static double Cos(Angle angle)
            => Math.Cos(angle.Radians);

        public static double Sin(Angle angle)
            => Math.Sin(angle.Radians);

        public static double Tan(Angle angle)
            => Math.Tan(angle.Radians);

        public static Angle Acos(double distance)
            => Math.Acos(distance);

        public static Angle Atan(double distance)
            => Math.Atan(distance);

        public static double Sqrt(double distance)
            => Math.Sqrt(distance);
    }
}
