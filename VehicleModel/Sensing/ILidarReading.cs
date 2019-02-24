using Racing.Mathematics;
using System.Collections.Generic;

namespace Racing.Model.Sensing
{
    public interface ILidarReading
    {
        Angle AngularResolution { get; }
        Length MaximumDistance { get; }
        Length[] Readings { get; }
        IEnumerable<Vector> ToPointCloud();
    }
}
