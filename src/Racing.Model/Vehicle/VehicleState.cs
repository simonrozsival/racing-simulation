using Racing.Mathematics;
using System;

namespace Racing.Model.Vehicle
{
    public readonly struct VehicleState : IEquatable<VehicleState>
    {
        public Vector Position { get; }
        public double HeadingAngle { get; }
        public double SteeringAngle { get; }
        public double Speed { get; }

        public VehicleState(
            Vector position,
            double headingAngle,
            double steeringAngle,
            double speed)
        {
            Position = position;
            HeadingAngle = headingAngle;
            SteeringAngle = steeringAngle;
            Speed = speed;
        }

        public override bool Equals(object obj)
            => (obj is VehicleState other) && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(Position, HeadingAngle, SteeringAngle, Speed);

        public bool Equals(VehicleState other)
            => (Position, HeadingAngle, SteeringAngle, Speed) == (other.Position, other.HeadingAngle, other.SteeringAngle, other.Speed);

        public override string ToString()
            => $"{Position}, heading: {HeadingAngle}, speed: {Speed}";
    }
}
