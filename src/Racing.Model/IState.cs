using Racing.Mathematics;

namespace Racing.Model
{
    public interface IState
    {
        Vector Position { get; }
        Angle HeadingAngle { get; }
        Angle SteeringAngle { get; }
        double Speed { get; }
    }
}
