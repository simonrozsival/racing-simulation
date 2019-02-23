using Racing.Mathematics;

namespace Racing.Model.Sensing
{
    public interface ILidar
    {
        ILidarReading Scan(Vector origin);
    }
}
