using CommandLine;

namespace Racing.Simulation
{
    internal sealed class Options
    {
        [Option('i', "track", Required = true, HelpText = "Path of JSON track definition file.")]
        public string Input { get; set; } = string.Empty;

        [Option('o', "results", Required = true, HelpText = "Directory where the results will be written.")]
        public string Output { get; set; } = string.Empty;

        [Option('n', "number-of-repetitions", Required = false, Default = 1, HelpText = "Number of simulation repetitions.")]
        public int NumberOfRepetitions { get; set; }
    }
}
