using Racing.Mathematics;

namespace Racing.Model
{
    public interface ILidar
    {
        Angle AngularResolution { get; }
        Distance MaximumDistance { get; }
        Distance[] Readings { get; }
    }
}
