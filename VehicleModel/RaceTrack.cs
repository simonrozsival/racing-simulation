using Racing.Mathematics;
using System.Collections.Generic;

namespace Racing.Model
{
    internal sealed class RaceTrack : ITrack
    {
        public double TileSize { get; }
        public ICircuit Circuit { get; }
        public bool[,] OccupancyGrid { get; }

        public RaceTrack(
            ICircuit circuit,
            bool[,] occupancyGrid,
            double tileSize)
        {
            Circuit = circuit;
            OccupancyGrid = occupancyGrid;
            TileSize = tileSize;
        }
    }
}
