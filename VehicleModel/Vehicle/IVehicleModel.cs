using RacePlanning.Model.Math;

namespace RacePlanning.Model.VehicleModel
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

        Rectangle CalculateBounds(VehicleState state);
    }
}
