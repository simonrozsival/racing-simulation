using Racing.Mathematics;
using System.Collections.Generic;

namespace Racing.Model
{
    internal sealed class RaceTrack
    {
        private readonly double minX;
        private readonly double minY;
        private readonly double maxX;
        private readonly double maxY;

        public double TileSize { get; }
        public bool[,] OccupancyGrid { get; }

        public RaceTrack(bool[,] occupancyGrid, double tileSize)
        {
            this.OccupancyGrid = occupancyGrid;

            TileSize = tileSize;

            minX = 0;
            minY = 0;
            maxX = occupancyGrid.GetLength(0) * tileSize;
            maxY = occupancyGrid.GetLength(1) * tileSize;
        }

        public bool IsCollision(IEnumerable<Point> points)
        {
            foreach (var point in points)
            {
                if (isOutOfBounds(point.X, point.Y))
                {
                    return true;
                }

                var tileX = (int)(point.X / TileSize);
                var tileY = (int)(point.Y / TileSize);

                if (!OccupancyGrid[tileX, tileY])
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsCollision(Point position, double size)
        {
            for (var dx = -1; dx <= 1; dx++)
            {
                for (var dy = -1; dy <= 1; dy++)
                {
                    var x = position.X + dx * size;
                    var y = position.Y + dy * size;

                    if (isOutOfBounds(x, y))
                    {
                        return true;
                    }

                    var tileX = (int)(x / TileSize);
                    var tileY = (int)(y / TileSize);

                    if (!OccupancyGrid[tileX, tileY])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool isOutOfBounds(double x, double y)
            => x < minX || y < minY || x >= maxX || y >= maxY;
    }
}
