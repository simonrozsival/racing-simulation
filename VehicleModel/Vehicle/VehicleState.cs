using Racing.Mathematics;
using Racing.Model.Vehicle;

namespace Racing.Model.VehicleModel
{
    internal class VehicleState : IState
    {
        public Point Position { get; }
        public Angle HeadingAngle { get; }
        public Angle SteeringAngle { get; }
        public double Velocity { get; }

        public VehicleState(
            Point position,
            Angle heading,
            Angle steering,
            double velocity)
        {
            Position = position;
            HeadingAngle = heading;
            SteeringAngle = steering;
            Velocity = velocity;
        }
    }
}
