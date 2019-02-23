using Racing.Mathematics;
using System.Collections.Generic;
using System.Text;

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

        public IEnumerable<Vector> CalculatePointCloud()
        {
            for (int i = 0; i < Readings.Length; i++)
            {
                yield return Vector.From(Readings[i], i * AngularResolution);
            }
        }
    }
}
