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
            maxX = track.OccupancyGrid.GetLength(0) * track.TileSize;
            maxY = track.OccupancyGrid.GetLength(1) * track.TileSize;
        }

        public bool IsOutOfBounds(double x, double y)
            => x < minX || y < minY || x >= maxX || y >= maxY;
    }
}
