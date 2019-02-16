using CommandLine;

namespace Racing.Simulation
{
    internal sealed class Options
    {
        [Option('i', "input", Required = true, HelpText = "Path of JSON track definition file.")]
        public string Input { get; set; }

        [Option('n', "number-of-repetitions", Required = false, Default = 1, HelpText = "Number of simulation repetitions.")]
        public int NumberOfRepetitions { get; set; }
    }
}
