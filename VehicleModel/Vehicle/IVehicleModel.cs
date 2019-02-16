using Racing.Mathematics;
using Racing.Model.Vehicle;

namespace Racing.Model.VehicleModel
{
    interface IVehicleModel
    {
        double Width { get; }
        double Length { get; }
        double MinVelocity { get; }
        double MaxVelocity { get; }
        Angle MinSteeringAngle { get; }
        Angle MaxSteeringAngle { get; }
        double Acceleration { get; }
        Angle SteeringAcceleration { get; }

        Rectangle CalculateBounds(IState state);
    }
}
