using Racing.Mathematics;

namespace Racing.Model.Sensing
{
    public interface ILidar
    {
        Length MaximumDistance { get; }
        ILidarReading Scan(Vector origin, Angle direction);
    }
}
