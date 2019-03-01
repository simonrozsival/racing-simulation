using Racing.Mathematics;
using Racing.Model;
using Racing.Model.Vehicle;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Racing.IO.Model
{
    internal sealed class SerializableTrack
    {
        public static SerializableTrack From(double tileSize, ICircuit circuit, bool[,] occupancyGrid)
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

            return new SerializableTrack
            {
                TileSize = tileSize,
                Circuit = new SerializableCircuit { Radius = circuit.Radius, Start = circuit.Start, WayPoints = circuit.WayPoints.Skip(1).Select(goal => goal.Position).ToList() },
                OccupancyGrid = occupancyGridLines.ToArray()
            };
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
                    occupancyGrid[i, j] = tmpOccupancyGrid[j][i];
                }
            }

            return new RaceTrack(
                new DeserialziedCircuit
                {
                    Start = new Vector(Circuit.Start.X, Circuit.Start.Y),
                    Radius = Circuit.Radius,
                    WayPoints = Circuit.WayPoints.Select(point => new RadialGoal(new Vector(point.X, point.Y), Circuit.Radius)).ToList<IGoal>()
                },
                occupancyGrid,
                TileSize);
        }

        private sealed class DeserialziedCircuit : ICircuit
        {
            public double Radius { get; set; }
            public Vector Start { get; set; }
            public IList<IGoal> WayPoints { get; set; } = new List<IGoal>();
            public VehicleState StartingPosition { get; set; }
        }
    }
}
