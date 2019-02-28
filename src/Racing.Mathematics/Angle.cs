using static System.Math;

namespace Racing.Mathematics
{
    public static class Angle
    {
        public static double Zero = FromDegrees(0);
        public static double FullCircle = FromDegrees(360);

        public static double FromDegrees(double degrees)
            => degrees / 180.0 * PI;

        public static double SmallAngle(double angle)
        {
            while (angle < 0) angle += 2 * PI;
            while (angle >= 2 * PI) angle -= 2 * PI;
            return angle;
        }
    }
}
