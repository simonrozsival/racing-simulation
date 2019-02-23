using Racing.Mathematics;

namespace Racing.Model.Sensing
{
    public interface ILidarReading
    {
        Angle AngularResolution { get; }
        Length MaximumDistance { get; }
        Length[] Readings { get; }
    }
}
