using CommandLine;
using Racing.CircuitGenerator.Output;
using Racing.IO;
using System;
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

            Console.WriteLine($"Generated {waypoints.Count} waypoints");

            var circuit = new Circuit(waypoints, options.TrackWidth);

            var imageGenerator = new ImageGenerator(options.Width, options.Height, circuit);
            var svg = imageGenerator.GenerateSvg();

            Console.WriteLine($"Generated SVG");

            var path = $"{options.OutputPath}/generated-at-{DateTimeOffset.Now.ToUnixTimeMilliseconds()}";
            Track.Save(path, circuit, svg, options.OccupancyGridResolution);

            Console.WriteLine($"Coverted to PNG");
        }
    }
}
