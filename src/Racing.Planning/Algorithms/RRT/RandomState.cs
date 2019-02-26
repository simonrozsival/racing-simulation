using Racing.Mathematics;
using Racing.Model;

namespace Racing.Planning.Algorithms.RRT
{
    internal sealed class RandomState : IState
    {
        public RandomState(Vector position, double headingAngle, double steeringAngle, double speed)
        {
            Position = position;
            HeadingAngle = headingAngle;
            SteeringAngle = steeringAngle;
            Speed = speed;
        }

        public Vector Position { get; }
        public double HeadingAngle { get; }
        public double SteeringAngle { get; }
        public double Speed { get; }
    }
}
