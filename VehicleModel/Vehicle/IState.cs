using Racing.Mathematics;

namespace Racing.Model.Vehicle
{
    public interface IState
    {
        Point Position { get; }
        Angle HeadingAngle { get; }
        Angle SteeringAngle { get; }
        double Velocity { get; }
    }
}
