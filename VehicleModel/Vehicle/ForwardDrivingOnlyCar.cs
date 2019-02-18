using Racing.Mathematics;

namespace Racing.Model.Vehicle
{
    public sealed class ForwardDrivingOnlyVehicle : IVehicleModel
    {
        public double Width { get; }
        public double Length { get; }
        public double MinVelocity { get; }
        public double MaxVelocity { get; }
        public Angle MinSteeringAngle { get; }
        public Angle MaxSteeringAngle { get; }
        public double Acceleration { get; }
        public Angle SteeringAcceleration { get; }

        public ForwardDrivingOnlyVehicle(double width)
        {
            var oneMeter = width / 1.85;
            Width = width;
            Length = 2 * width;
            MinVelocity = 0;
            MaxVelocity = 27 * oneMeter;
            MinSteeringAngle = Angle.FromDegrees(-25);
            MaxSteeringAngle = Angle.FromDegrees(25);
            Acceleration = 16 * oneMeter;
            SteeringAcceleration = MaxSteeringAngle / 0.5;
        }
    }
}
