using Racing.Mathematics;

namespace Racing.Model.Vehicle
{
    public sealed class ForwardDrivingOnlyVehicle : IVehicleModel
    {
        public double Width { get; }
        public double Length { get; }
        public double MinSpeed { get; }
        public double MaxSpeed { get; }
        public double MinSteeringAngle { get; }
        public double MaxSteeringAngle { get; }
        public double Acceleration { get; }
        public double SteeringAcceleration { get; }

        public ForwardDrivingOnlyVehicle(double width)
        {
            var oneMeter = width / 1.85;
            Width = width;
            Length = 2 * width;
            MinSpeed = 0;
            MaxSpeed = 27 * oneMeter;
            MinSteeringAngle = Angle.FromDegrees(-25);
            MaxSteeringAngle = Angle.FromDegrees(25);
            Acceleration = 16 * oneMeter;
            SteeringAcceleration = MaxSteeringAngle / 0.5;
        }
    }
}
