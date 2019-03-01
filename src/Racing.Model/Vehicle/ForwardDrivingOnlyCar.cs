﻿using Racing.Mathematics;

namespace Racing.Model.Vehicle
{
    public sealed class ForwardDrivingOnlyVehicle : IVehicleModel
    {
        public double Width { get; }
        public double Length { get; }
        public double MinSpeed { get; } = 0;
        public double MaxSpeed { get; }
        public double MaxSteeringAngle { get; } = Angle.FromDegrees(30);
        public double Acceleration { get; }
        public double SteeringAcceleration { get; }
        public double BrakingDeceleration { get; }
        public double Mass { get; }

        public ForwardDrivingOnlyVehicle(double width)
        {
            // Porsche 911 is 1.85m wide and 4.5m long
            var oneMeter = width / 1.85;
            Width = 1.85 * oneMeter;
            Length = 4.5 * oneMeter;
            MaxSpeed = 54 * oneMeter; // 27 m/s == 100 km/h
            Acceleration = 9 * oneMeter; // 9 ms^-2 => 0-100 km/h in 3s
            SteeringAcceleration = 8 * MaxSteeringAngle;
            BrakingDeceleration = -27 * oneMeter;
            Mass = 1850; // kg
        }
    }
}
