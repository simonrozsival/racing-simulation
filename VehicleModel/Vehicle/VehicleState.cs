using RacePlanning.Model.Math;

namespace RacePlanning.Model.VehicleModel
{
    internal class VehicleState
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
