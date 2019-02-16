using Racing.Model.Math;
using Racing.Model.Vehicle;

namespace Racing.Model.VehicleModel
{
    interface IVehicleModel
    {
        double Width { get; }
        double Length { get; }
        double MinVelocity { get; }
        double MaxVelocity { get; }
        double MinSteeringAngle { get; }
        double MaxSteeringAngle { get; }
        double Acceleration { get; }
        double SteeringAcceleration { get; }

        Rectangle CalculateBounds(IState state);
    }
}
