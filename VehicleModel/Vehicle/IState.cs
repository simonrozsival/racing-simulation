using Racing.Model.Math;

namespace Racing.Model.Vehicle
{
    public interface IState
    {
        Point Position { get; }
        double HeadingAngle { get; }
        double SteeringAngle { get; }
        double Velocity { get; }
    }
}
