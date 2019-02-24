using System;

namespace Racing.Planning.Algorithms.RRT
{
    public static class RandomExtensions
    {
        public static double NextDoubleBetween(this Random random, double min, double max)
            => min + random.NextDouble() * (max - min);
    }
}
