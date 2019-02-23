using Racing.Mathematics;

namespace Racing.Model
{
    public sealed class RaceTrack : ITrack
    {
        public Distance TileSize { get; }
        public ICircuit Circuit { get; }
        public bool[,] OccupancyGrid { get; }
        public Distance Width { get; }
        public Distance Height { get; }

        public RaceTrack(
            ICircuit circuit,
            bool[,] occupancyGrid,
            Distance tileSize)
        {
            Circuit = circuit;
            OccupancyGrid = occupancyGrid;
            TileSize = tileSize;
            Width = OccupancyGrid.GetLength(0) * tileSize;
            Height = OccupancyGrid.GetLength(1) * tileSize;
        }

        public bool IsOccupied(double x, double y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
            {
                return true;
            }

            var tileX = (int)(x / TileSize.Meters);
            var tileY = (int)(y / TileSize.Meters);
            return !OccupancyGrid[tileX, tileY];
        }
    }
}
