using Racing.Mathematics;

namespace Racing.IO.Model
{
    internal readonly struct SerializableVector
    {
        public double X { get; }
        public double Y { get; }

        public SerializableVector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator SerializableVector(Vector vector)
            => new SerializableVector(vector.X.Meters, vector.Y.Meters);
    }
}
