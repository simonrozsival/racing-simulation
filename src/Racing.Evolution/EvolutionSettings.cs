namespace Racing.Evolution
{
    public sealed class EvolutionSettings
    {
        public int PopulationSize { get; set; }

        public int SpeciesCount { get; set; }

        public double ElitismProportion { get; set; }

        public int ComplexityThreshold { get; set; }
    }
}
