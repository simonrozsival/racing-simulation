using Racing.Mathematics;
using Racing.Model;
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
            double width,
            double length,
            double minVelocity,
            double maxVelocity,
            Angle minSteeringAngle,
            Angle maxSteeringAngle,
            double acceleration,
            Angle steeringAcceleration,
            double bias,
            Random random)
        {
            Width = width + bias * width * random.NextDouble();
            Length = length + bias * length * random.NextDouble();
            MinVelocity = minVelocity + bias * minVelocity * random.NextDouble();
            MaxVelocity = maxVelocity - bias * maxVelocity * random.NextDouble();
            MinSteeringAngle = minSteeringAngle + bias * minSteeringAngle * random.NextDouble();
            MaxSteeringAngle = maxSteeringAngle + bias * maxSteeringAngle * random.NextDouble();
            Acceleration = acceleration + bias * acceleration * random.NextDouble();
            SteeringAcceleration = steeringAcceleration + bias * steeringAcceleration * random.NextDouble();
        }
    }
}
