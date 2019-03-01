using Racing.Mathematics;
using System;

namespace Racing.Model.Vehicle
{
    public readonly struct VehicleState : IEquatable<VehicleState>
    {
        public Vector Position { get; }
        public double HeadingAngle { get; }
        public double Speed { get; }
        public double AngularVelocity { get; }

        public VehicleState(
            Vector position,
            double headingAngle,
            double speed,
            double angularVelocity)
        {
            Position = position;
            HeadingAngle = headingAngle;
            Speed = speed;
            AngularVelocity = angularVelocity;
        }

        public override bool Equals(object obj)
            => (obj is VehicleState other) && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(Position, HeadingAngle, Speed, AngularVelocity);

        public bool Equals(VehicleState other)
            => (Position, HeadingAngle, Speed, AngularVelocity) == (other.Position, other.HeadingAngle, other.Speed, AngularVelocity);

        public override string ToString()
            => $"{Position}, heading: {HeadingAngle}, speed: {Speed}";
    }
}
