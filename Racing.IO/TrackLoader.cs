using Racing.Model;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Racing.IO.Model;

namespace Racing.IO
{
    public static class TrackLoader
    {
        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public static void Save(string path, ICircuit circut, string svg, double tileSize)
        {
            Directory.CreateDirectory(path);

            // svg
            var svgFileName = $"{path}/visualization.svg";
            File.WriteAllText(svgFileName, svg);

            // png
            var pngFileName = $"{path}/rasterized_visualization.png";
            Images.Convert(svgFileName, pngFileName);

            // occupancy grid
            var occupancyGrid = Images.LoadOccupancyGrid(pngFileName, tileSize);

            // json
            var track = new Track
            {
                Circuit = circut,
                OccupancyGrid = occupancyGrid,
                TileSize = tileSize
            };
            var jsonFileName = $"{path}/circuit_definition.json";
            var json = JsonConvert.SerializeObject(track, settings);
            File.WriteAllText(path, json);
        }

        public static ITrack Load(string path)
        {
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<Track>(json, settings);
        }
    }
}
