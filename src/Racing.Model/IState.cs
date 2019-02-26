using Racing.Mathematics;

namespace Racing.Model
{
    public interface IState
    {
        Vector Position { get; }
        double HeadingAngle { get; }
        double SteeringAngle { get; }
        double Speed { get; }
    }
}
