namespace Racing.Model
{
    public sealed class RaceTrack : ITrack
    {
        public double TileSize { get; }
        public ICircuit Circuit { get; }
        public bool[,] OccupancyGrid { get; }
        public double Width { get; }
        public double Height { get; }

        public RaceTrack(
            ICircuit circuit,
            bool[,] occupancyGrid,
            double tileSize)
        {
            Circuit = circuit;
            OccupancyGrid = occupancyGrid;
            TileSize = tileSize;
            Width = OccupancyGrid.GetLength(0) * tileSize;
            Height = OccupancyGrid.GetLength(1) * tileSize;
        }

        public bool IsOccupied(double x, double y)
        {
            var tileX = (int)(x / TileSize);
            var tileY = (int)(y / TileSize);
            return !OccupancyGrid[tileX, tileY];
        }
    }
}
