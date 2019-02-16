using CommandLine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Racing.CircuitGenerator.Output;
using Racing.CircuitGenerator.Splines;
using System;
using System.Diagnostics;
using System.DrawingCore;
using System.IO;
using System.Linq;

namespace Racing.CircuitGenerator
{
    internal sealed class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(run);
        }

        private static void run(Options options)
        {
            var random = options.Random
                ? new Random()
                : new Random(options.Seed);

            for (int i = 0; i < options.NumberOfTracks; i++)
            {
                Console.WriteLine($"Generating Track #{i}:");
                generateTrack(options, random);
                Console.WriteLine("=================.\n");
            }
        }

        private static void generateTrack(Options options, Random random)
        {
            var waypointsGenerator = new WaypointsGenerator(
                options.MinimumWaypointsCount, options.MaximumWaypointsCount, random, 1.5 * options.TrackWidth);
            var waypoints = waypointsGenerator.Generate(options.Width, options.Height).ToList();
            // var waypoints = waypointsGenerator.GenerateSimpleCircuit(options.Width, options.Height).ToList();
            var circuit = new Circuit(waypoints, options.TrackWidth);

            var imageGenerator = new ImageGenerator(options.Width, options.Height, circuit);

            var outputDirectory = $"{options.OutputPath}/generated-at-{DateTimeOffset.Now.ToUnixTimeMilliseconds()}";
            Directory.CreateDirectory(outputDirectory);

            // svg
            var svgFileName = $"{outputDirectory}/visualization.svg";
            var svg = imageGenerator.GenerateSvg();
            File.WriteAllText(svgFileName, svg);
            Console.WriteLine($"Generated SVG: {svgFileName}");

            // png
            var pngFileName = $"{outputDirectory}/rasterized_visualization.png";
            generatePng(svgFileName, pngFileName);
            Console.WriteLine($"Generated PNG: {pngFileName}");

            // occupancy grid
            var occupancyGrid = occupancyGridFrom(pngFileName, options.OccupancyGridResolution);

            // write json
            var trackDefinition = new TrackDefinition
            {
                Circuit = circuit,
                OccupancyGrid = occupancyGrid
            };

            var serializedTrack = JsonConvert.SerializeObject(
                trackDefinition,
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

            var jsonFileName = $"{outputDirectory}/circuit_definition.json";
            File.WriteAllText(jsonFileName, serializedTrack);
            Console.WriteLine($"Generated JSON: {jsonFileName}");
        }

        private static void generatePng(string svgPath, string pngPath)
        {
            var info = new ProcessStartInfo
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "svgexport",
                Arguments = $"{svgPath} {pngPath}",
                UseShellExecute = true
            };

            using (var process = Process.Start(info))
            {
                process.WaitForExit();
            }
        }

        private static bool[,] occupancyGridFrom(string pngFile, double resolution)
        {
            var occupancyGridGenerator = new OccupancyGridGenerator(resolution);
            using (var stream = File.OpenRead(pngFile))
            {
                using (var image = Image.FromStream(stream))
                {
                    var bitmap = new Bitmap(image);
                    return occupancyGridGenerator.GenerateOccupancyGrid(bitmap);
                }
            }
        }
    }
}
