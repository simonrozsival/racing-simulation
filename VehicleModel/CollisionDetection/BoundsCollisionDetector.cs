using Racing.Mathematics;
using System;

namespace Racing.Model.CollisionDetection
{
    internal class BoundsDetector
    {
        private readonly double minX;
        private readonly double minY;
        private readonly double maxX;
        private readonly double maxY;

        public BoundsDetector(ITrack track)
        {
            minX = 0;
            minY = 0;
            maxX = track.Width;
            maxY = track.Height;
        }

        public bool IsOutOfBounds(double x, double y)
            => x < minX || y < minY || x >= maxX || y >= maxY;
    }
}
