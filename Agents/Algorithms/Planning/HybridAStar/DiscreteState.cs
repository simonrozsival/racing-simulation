using System;

namespace Racing.Agents.Algorithms.Planning.HybridAStar
{
    internal readonly struct DiscreteState : IEquatable<DiscreteState>
    {
        public int X { get; }
        public int Y { get; }
        public int HeadingAngle { get; }
        public int RemainingWaypointsCount { get; }

        public DiscreteState(int x, int y, int headingAngle, int remainingWaypointsCount) : this()
        {
            X = x;
            Y = y;
            HeadingAngle = headingAngle;
            RemainingWaypointsCount = remainingWaypointsCount;
        }

        public override bool Equals(object obj)
            => (obj is DiscreteState other) && Equals(other);

        public override int GetHashCode()
            => (X, Y, HeadingAngle, RemainingWaypointsCount).GetHashCode();

        public override string ToString()
            => FormattableString.Invariant($"[{X}, {Y}], θ: {HeadingAngle}");

        public bool Equals(DiscreteState other)
            => (X, Y, HeadingAngle, RemainingWaypointsCount) == (other.X, other.Y, other.HeadingAngle, other.RemainingWaypointsCount);

        public static bool operator ==(DiscreteState a, DiscreteState b)
            => a.Equals(b);

        public static bool operator !=(DiscreteState a, DiscreteState b)
            => !a.Equals(b);
    }
}
