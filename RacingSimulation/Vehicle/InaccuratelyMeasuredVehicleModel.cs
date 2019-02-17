using Racing.Mathematics;
using Racing.Model.Vehicle;
using System;

namespace Racing.Simulation.Vehicle
{
    internal sealed class InaccuratelyMeasuredVehicleModel : IVehicleModel
    {
        public double Width { get; }
        public double Length { get; }
        public double MinVelocity { get; }
        public double MaxVelocity { get; }
        public Angle MinSteeringAngle { get; }
        public Angle MaxSteeringAngle { get; }
        public double Acceleration { get; }
        public Angle SteeringAcceleration { get; }

        public InaccuratelyMeasuredVehicleModel(
            IVehicleModel measuredVehicleModel,
            double bias,
            Random random)
        {
            Width = measuredVehicleModel.Width + bias * measuredVehicleModel.Width * random.NextDouble();
            Length = measuredVehicleModel.Length + bias * measuredVehicleModel.Length * random.NextDouble();
            MinVelocity = measuredVehicleModel.MinVelocity + bias * measuredVehicleModel.MinVelocity * random.NextDouble();
            MaxVelocity = measuredVehicleModel.MaxVelocity - bias * measuredVehicleModel.MaxVelocity * random.NextDouble();
            MinSteeringAngle = measuredVehicleModel.MinSteeringAngle + bias * measuredVehicleModel.MinSteeringAngle * random.NextDouble();
            MaxSteeringAngle = measuredVehicleModel.MaxSteeringAngle + bias * measuredVehicleModel.MaxSteeringAngle * random.NextDouble();
            Acceleration = measuredVehicleModel.Acceleration + bias * measuredVehicleModel.Acceleration * random.NextDouble();
            SteeringAcceleration = measuredVehicleModel.SteeringAcceleration + bias * measuredVehicleModel.SteeringAcceleration * random.NextDouble();
        }
    }
}
