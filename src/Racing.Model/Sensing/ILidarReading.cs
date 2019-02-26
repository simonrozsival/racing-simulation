using Racing.Mathematics;
using System.Collections.Generic;

namespace Racing.Model.Sensing
{
    public interface ILidarReading
    {
        double AngularResolution { get; }
        double MaximumDistance { get; }
        double[] Readings { get; }
        IEnumerable<Vector> ToPointCloud();
    }
}
