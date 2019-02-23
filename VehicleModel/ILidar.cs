using Racing.Mathematics;

namespace Racing.Model
{
    public interface ILidar
    {
        Angle AngularResolution { get; }
        Length MaximumDistance { get; }
        Length[] Readings { get; }
    }
}
