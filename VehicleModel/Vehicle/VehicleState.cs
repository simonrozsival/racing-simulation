using Racing.Model.Math;
using Racing.Model.Vehicle;

namespace Racing.Model.VehicleModel
{
    internal class VehicleState : IState
    {
        public Point Position { get; }
        public double HeadingAngle { get; }
        public double SteeringAngle { get; }
        public double Velocity { get; }

        public VehicleState(
            Point position,
            double heading,
            double steering,
            double velocity)
        {
            Position = position;
            HeadingAngle = heading;
            SteeringAngle = steering;
            Velocity = velocity;
        }
    }
}
