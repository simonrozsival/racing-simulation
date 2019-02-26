using Racing.Mathematics;
using System.Collections.Generic;

namespace Racing.Model.Sensing
{
    public interface ILidarReading
    {
        Angle AngularResolution { get; }
        double MaximumDistance { get; }
        double[] Readings { get; }
        IEnumerable<Vector> ToPointCloud();
    }
}
