using RaceCircuitGenerator.Splines;

namespace RaceCircuitGenerator.Output
{
    internal sealed class TrackDefinition
    {
        public Circuit Circuit { get; set; }
        public string[] OccupancyGrid { get; set; }
    }
}
