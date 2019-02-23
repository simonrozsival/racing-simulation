using Racing.Mathematics;
using Racing.Model;

namespace Racing.Agents.Algorithms.Planning.RRT
{
    internal sealed class RandomState : IState
    {
        public RandomState(Vector position, Angle headingAngle, Angle steeringAngle, double speed)
        {
            Position = position;
            HeadingAngle = headingAngle;
            SteeringAngle = steeringAngle;
            Speed = speed;
        }

        public Vector Position { get; }
        public Angle HeadingAngle { get; }
        public Angle SteeringAngle { get; }
        public double Speed { get; }
    }
}
