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

        public static Angle Acos(Length distance)
            => Math.Acos(distance.Meters);

        public static Angle Atan(Length distance)
            => Math.Atan(distance.Meters);

        public static Length Sqrt(Length distance)
            => Math.Sqrt(distance.Meters);
    }
}
