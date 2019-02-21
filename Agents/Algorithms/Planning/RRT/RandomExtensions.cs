using System;

namespace Racing.Agents.Algorithms.Planning.RRT
{
    public static class RandomExtensions
    {
        public static double NextDoubleBetween(this Random random, double min, double max)
            => min + random.NextDouble() * (max - min);
    }
}
