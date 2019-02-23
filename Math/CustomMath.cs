using System;
using static System.Math;

namespace Racing.Mathematics
{
    public static class CustomMath
    {
        public static double Cos(Angle angle)
            => Cos(angle.Radians);

        public static double Sin(Angle angle)
            => Sin(angle.Radians);

        public static double Tan(Angle angle)
            => Tan(angle.Radians);

        public static Angle Acos(Length distance)
            => Acos(distance.Meters);

        public static Angle Atan(Length distance)
            => Atan(distance.Meters);

        public static Length Sqrt(Length distance)
            => Sqrt(distance.Meters);
    }
}
