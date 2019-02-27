using Racing.Mathematics;

namespace Racing.Model.Vehicle
{
    public interface IVehicleModel
    {
        double Width { get; }
        double Length { get; }
        double MinSpeed { get; }
        double MaxSpeed { get; }
        double MaxSteeringAngle { get; }
        double Acceleration { get; }
        double SteeringAcceleration { get; }
        double BrakingDeceleration { get; }
    }
}
