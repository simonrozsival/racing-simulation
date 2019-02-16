using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace Racing.CircuitGenerator.Output
{
    internal sealed class TrackDefinition : ITrackDefinition
    {
        public ICircuit Circuit { get; set; }

        [JsonIgnore]
        public bool[,] OccupancyGrid { get; set; }

        [JsonProperty(PropertyName = "occupancyGrid")]
        public string[] OccupancyGridSerialization
        {
            get
            {
                var lines = new List<string>();

                for (int x = 0; x < OccupancyGrid.GetLength(0); x++)
                {
                    var builder = new StringBuilder();
                    for (int y = 0; y < OccupancyGrid.GetLength(1); y++)
                    {
                        var symbol = OccupancyGrid[x, y] ? ' ' : '#';
                        builder.Append(symbol);
                    }

                    lines.Add(builder.ToString());
                }

                return lines.ToArray();
            }

            set
            {
                var height = value.Length;
                var width = value[0].Length;
                OccupancyGrid = new bool[width, height];

                for (int y = 0; y < height; y++)
                {
                    var symbols = value[y];
                    for (int x = 0; x < width; x++)
                    {
                        OccupancyGrid[x, y] = symbols[x] == ' ';
                    }
                }
            }
        }
    }
}
