using Racing.Mathematics;

namespace Racing.Model
{
    public interface ILidar
    {
        Angle AngularResolution { get; }
        double MaximumDistance { get; }
        double[] Readings { get; }
    }
}
