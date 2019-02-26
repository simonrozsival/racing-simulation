using Racing.Mathematics;
using System.Collections.Generic;

namespace Racing.Model.Sensing
{
    internal sealed class LidarReading : ILidarReading
    {
        public LidarReading(
            double angularResolution, 
            double startAngle,
            double maximumDistance,
            double[] readings)
        {
            AngularResolution = angularResolution;
            StartAngle = startAngle;
            MaximumDistance = maximumDistance;
            Readings = readings;
        }

        public double AngularResolution { get; }
        public double StartAngle { get; }
        public double MaximumDistance { get; }
        public double[] Readings { get; }

        public IEnumerable<Vector> ToPointCloud()
        {
            for (int i = 0; i < Readings.Length; i++)
            {
                var distance = Readings[i];
                if (distance == double.MaxValue)
                {
                    distance = MaximumDistance;
                }

                yield return Vector.From(distance, i * AngularResolution);
            }
        }
    }
}
