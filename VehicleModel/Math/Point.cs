using static System.Math;

namespace RacePlanning.Model.Math
{
    internal sealed class Point
    {
        public double X { get; }
        public double Y { get; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Point Rotate(Point center, double angle)
        {
            var dx = X - center.X;
            var dy = Y - center.Y;
            return new Point(
                x: dx * Cos(angle) - dy * Sin(angle) + center.X,
                y: dy * Cos(angle) + dx * Sin(angle) + center.Y);
        }

        public double DistanceTo(Point other)
        {
            var dx = X - other.X;
            var dy = Y - other.Y;
            return Sqrt(dx * dx + dy * dy);
        }
    }
}
