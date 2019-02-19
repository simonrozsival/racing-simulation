using Racing.Mathematics;
using System;

namespace Racing.Model.Vehicle
{
    internal readonly struct VehicleState : IState, IEquatable<VehicleState>
    {
        public Point Position { get; }
        public Angle HeadingAngle { get; }
        public Angle SteeringAngle { get; }
        public double Velocity { get; }

        public VehicleState(
            Point position,
            Angle heading,
            Angle steering,
            double velocity)
        {
            Position = position;
            HeadingAngle = heading;
            SteeringAngle = steering;
            Velocity = velocity;
        }

        public override bool Equals(object obj)
            => (obj is VehicleState other) && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(Position, HeadingAngle, SteeringAngle, Velocity);

        public bool Equals(VehicleState other)
            => (Position, HeadingAngle, SteeringAngle, Velocity) == (other.Position, other.HeadingAngle, other.SteeringAngle, other.Velocity);
    }
}
