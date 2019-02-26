using static System.Math;

namespace Racing.Mathematics
{
    public static class Angle
    {
        public static double Zero = FromDegrees(0);
        public static double FullCircle = FromDegrees(360);

        public static double FromDegrees(double degrees)
            => degrees / 180.0 * PI;
    }
}
