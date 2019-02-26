using Racing.Mathematics;

namespace Racing.Model.Sensing
{
    public interface ILidar
    {
        double MaximumDistance { get; }
        ILidarReading Scan(Vector origin, Angle direction);
    }
}
