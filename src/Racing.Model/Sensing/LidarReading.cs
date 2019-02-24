using Racing.Mathematics;
using System.Collections.Generic;

namespace Racing.Model.Sensing
{
    internal sealed class LidarReading : ILidarReading
    {
        public LidarReading(Angle angularResolution, Length maximumDistance, Length[] readings)
        {
            AngularResolution = angularResolution;
            MaximumDistance = maximumDistance;
            Readings = readings;
        }

        public Angle AngularResolution { get; }
        public Length MaximumDistance { get; }
        public Length[] Readings { get; }

        public IEnumerable<Vector> ToPointCloud()
        {
            for (int i = 0; i < Readings.Length; i++)
            {
                var distance = Readings[i];
                if (distance == Length.Infinite)
                {
                    distance = MaximumDistance;
                }

                yield return Vector.From(distance, i * AngularResolution);
            }
        }
    }
}
