using Racing.Mathematics;
using Racing.Model;

namespace Racing.Agents.Algorithms.Planning.RRT
{
    internal sealed class RandomState : IState
    {
        public RandomState(Point position, Angle headingAngle, Angle steeringAngle, double speed)
        {
            Position = position;
            HeadingAngle = headingAngle;
            SteeringAngle = steeringAngle;
            Speed = speed;
        }

        public Point Position { get; }
        public Angle HeadingAngle { get; }
        public Angle SteeringAngle { get; }
        public double Speed { get; }
    }
}
