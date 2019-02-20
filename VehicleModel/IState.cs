using Racing.Mathematics;

namespace Racing.Model
{
    public interface IState
    {
        Point Position { get; }
        Angle HeadingAngle { get; }
        Angle SteeringAngle { get; }
        double Speed { get; }
    }
}
