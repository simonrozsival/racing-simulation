using Racing.Mathematics;
using System;

namespace Racing.Model.Vehicle
{
    internal readonly struct VehicleState : IState, IEquatable<VehicleState>
    {
        public Point Position { get; }
        public Angle HeadingAngle { get; }
        public Angle SteeringAngle { get; }
        public double Speed { get; }

        public VehicleState(
            Point position,
            Angle heading,
            Angle steeringAngle,
            double speed)
        {
            Position = position;
            HeadingAngle = heading;
            SteeringAngle = steeringAngle;
            Speed = speed;
        }

        public override bool Equals(object obj)
            => (obj is VehicleState other) && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(Position, HeadingAngle, SteeringAngle, Speed);

        public bool Equals(VehicleState other)
            => (Position, HeadingAngle, SteeringAngle, Speed) == (other.Position, other.HeadingAngle, other.SteeringAngle, other.Speed);
    }
}
