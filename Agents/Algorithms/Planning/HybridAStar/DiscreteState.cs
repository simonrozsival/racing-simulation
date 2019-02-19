using System;

namespace Racing.Agents.Algorithms.Planning.HybridAStar
{
    internal readonly struct DiscreteState : IEquatable<DiscreteState>
    {
        public int X { get; }
        public int Y { get; }
        public int HeadingAngle { get; }

        public DiscreteState(int x, int y, int headingAngle) : this()
        {
            X = x;
            Y = y;
            HeadingAngle = headingAngle;
        }

        public override bool Equals(object obj)
            => (obj is DiscreteState other) && Equals(other);

        public override int GetHashCode()
            => (X, Y, HeadingAngle).GetHashCode();

        public override string ToString()
            => FormattableString.Invariant($"[{X}, {Y}], θ: {HeadingAngle}");

        public bool Equals(DiscreteState other)
            => (X, Y, HeadingAngle) == (other.X, other.Y, other.HeadingAngle);

        public static bool operator ==(DiscreteState a, DiscreteState b)
            => a.Equals(b);

        public static bool operator !=(DiscreteState a, DiscreteState b)
            => !a.Equals(b);
    }
}
