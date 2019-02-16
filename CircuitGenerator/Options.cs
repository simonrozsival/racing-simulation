using CommandLine;

namespace RaceCircuitGenerator
{
    internal sealed class Options
    {
        [Option("output", Required = true, HelpText = "Output directory.")]
        public string OutputPath { get; set; }

        [Option('w', "width", Required = false, HelpText = "Width of the track in meters.", Default = 1000)]
        public int Width { get; set; }

        [Option('h', "height", Required = false, HelpText = "Height of the track in meters.", Default = 1000)]
        public int Height { get; set; }

        [Option('m', "min-waypoints", Required = false, HelpText = "Minimum waypoints count.", Default = 10)]
        public int MinimumWaypointsCount { get; set; }

        [Option('M', "max-waypoints", Required = false, HelpText = "Maximum waypoints count.", Default = 20)]
        public int MaximumWaypointsCount { get; set; }

        [Option('t', "track-width", Required = false, HelpText = "Track width.", Default = 80)]
        public int TrackWidth { get; set; }

        [Option('r', "random", Required = false, HelpText = "Ignore the seed parameter.", Default = true)]
        public bool Random { get; set; }

        [Option('s', "seed", Required = false, HelpText = "Seed for the random number generator.", Default = 0)]
        public int Seed { get; set; }

        [Option('o', "occupancy-grid-resolution", Required = false, HelpText = "Occupancy grid sampling resolution.", Default = 5)]
        public double OccupancyGridResolution { get; set; }

        [Option('n', "number-of-tracks", Required = false, HelpText = "Number of generated tracks.", Default = 1)]
        public int NumberOfTracks { get; set; }
    }
}
