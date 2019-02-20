using Newtonsoft.Json;
using Racing.Mathematics;
using Racing.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Racing.IO.Model
{
    internal sealed class SerializableTrack
    {
        public SerializableTrack(double tileSize, ICircuit circuit, bool[,] occupancyGrid)
        {
            var occupancyGridLines = new List<string>();
            for (int i = 0; i < occupancyGrid.GetLength(0); i++)
            {
                var line = new StringBuilder();
                for (int j = 0; j < occupancyGrid.GetLength(1); j++)
                {
                    line.Append(occupancyGrid[i, j] ? ' ' : '#');
                }
                occupancyGridLines.Add(line.ToString());
            }

            TileSize = tileSize;
            Circuit = new SerializableCircuit { Goal = circuit.Goal, Radius = circuit.Radius, Start = circuit.Start, WayPoints = circuit.WayPoints };
            OccupancyGrid = occupancyGridLines.ToArray();
        }

        public double TileSize { get; set; }

        public SerializableCircuit Circuit { get; set; } = new SerializableCircuit();

        public string[] OccupancyGrid { get; set; } = new string[0];

        public ITrack ToTrack()
        {
            var tmpOccupancyGrid = OccupancyGrid.Select(line => line.ToCharArray().Select(symbol => symbol == ' ').ToArray()).ToArray();
            var occupancyGrid = new bool[tmpOccupancyGrid[0].Length, tmpOccupancyGrid.Length];
            for (int i = 0; i < occupancyGrid.GetLength(0); i++)
            {
                for (int j = 0; j < occupancyGrid.GetLength(1); j++)
                {
                    occupancyGrid[i, j] = tmpOccupancyGrid[i][j];
                }
            }

            return new RaceTrack(
                new DeserialziedCircuit { Goal = Circuit.Goal, Start = Circuit.Start, Radius = Circuit.Radius, WayPoints = Circuit.WayPoints },
                occupancyGrid,
                TileSize);
        }

        private sealed class DeserialziedCircuit : ICircuit
        {
            public double Radius { get; set; }
            public Point Start { get; set; }
            public Point Goal { get; set; }
            public IList<Point> WayPoints { get; set; } = new List<Point>();
        }
    }
}
