using Racing.Mathematics;

namespace Racing.Model
{
    public sealed class RaceTrack : ITrack
    {
        public Length TileSize { get; }
        public ICircuit Circuit { get; }
        public bool[,] OccupancyGrid { get; }
        public Length Width { get; }
        public Length Height { get; }

        public RaceTrack(
            ICircuit circuit,
            bool[,] occupancyGrid,
            Length tileSize)
        {
            Circuit = circuit;
            OccupancyGrid = occupancyGrid;
            TileSize = tileSize;
            Width = OccupancyGrid.GetLength(0) * tileSize;
            Height = OccupancyGrid.GetLength(1) * tileSize;
        }

        public bool IsOccupied(Vector position)
            => IsOccupied(position.X, position.Y);

        public bool IsOccupied(Length x, Length y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
            {
                return true;
            }

            var tileX = (int)(x / TileSize).Meters;
            var tileY = (int)(y / TileSize).Meters;
            return !OccupancyGrid[tileX, tileY];
        }
    }
}
