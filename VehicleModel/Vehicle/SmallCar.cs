using Racing.Mathematics;
using Racing.Model.Vehicle;

namespace Racing.Model.VehicleModel
{
    internal sealed class SmallCar : IVehicleModel
    {
        public double Width { get; }
        public double Length { get; }
        public double MinVelocity { get; }
        public double MaxVelocity { get; }
        public Angle MinSteeringAngle { get; }
        public Angle MaxSteeringAngle { get; }
        public double Acceleration { get; }
        public Angle SteeringAcceleration { get; }

        public SmallCar(
            double width,
            double length,
            double minVelocity,
            double maxVelocity,
            Angle minSteeringAngle,
            Angle maxSteeringAngle,
            double acceleration,
            Angle steeringAcceleration)
        {
            Width = width;
            Length = length;
            MinVelocity = minVelocity;
            MaxVelocity = maxVelocity;
            MinSteeringAngle = minSteeringAngle;
            MaxSteeringAngle = maxSteeringAngle;
            Acceleration = acceleration;
            SteeringAcceleration = steeringAcceleration;
        }

        public Rectangle CalculateBounds(IState state)
        {
            var ux = Length / 2;
            var uy = Width / 2;
            var alignedRect = new Rectangle(
                new Point(state.Position.X - ux, state.Position.Y + uy),
                new Point(state.Position.X - ux, state.Position.Y - uy),
                new Point(state.Position.X + ux, state.Position.Y - uy),
                new Point(state.Position.X + ux, state.Position.Y + uy));

            return alignedRect.Rotate(state.Position, state.HeadingAngle);
        }
    }
}
