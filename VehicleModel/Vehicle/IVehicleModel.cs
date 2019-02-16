using Racing.Mathematics;

namespace Racing.Model.Vehicle
{
    public interface IVehicleModel
    {
        double Width { get; }
        double Length { get; }
        double MinVelocity { get; }
        double MaxVelocity { get; }
        Angle MinSteeringAngle { get; }
        Angle MaxSteeringAngle { get; }
        double Acceleration { get; }
        Angle SteeringAcceleration { get; }
    }
}
