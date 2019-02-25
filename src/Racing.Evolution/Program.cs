using Racing.IO;
using Racing.Mathematics;
using Racing.Model;
using Racing.Model.Sensing;
using SharpNeat.Core;
using SharpNeat.Decoders;
using SharpNeat.Decoders.Neat;
using SharpNeat.DistanceMetrics;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using SharpNeat.SpeciationStrategies;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Racing.Evolution
{
    class Program
    {
        private const string assetsPath = "../../../../../assets";

        public static void Main(string[] args)
        {
            var circuitName = "generated-at-1550822778155";
            var circuitPath = Path.GetFullPath($"{assetsPath}/tracks/{circuitName}");

            var perceptionStep = TimeSpan.FromSeconds(0.1);
            var simulationStep = TimeSpan.FromSeconds(0.016); // 60Hz
            var maximumSimulationTime = TimeSpan.FromSeconds(60);

            var track = Track.Load($"{circuitPath}/circuit_definition.json");
            var world = new StandardWorld(track, simulationStep);

            var inputSamplesCount = 12;
            var maximumScanningDistance = Length.FromMeters(100);

            var settings = new EvolutionSettings
            {
                PopulationSize = 150,
                SpeciesCount = 10,
                ElitismProportion = 0.05,
                ComplexityThreshold = 50
            };

            // prepare simulation
            var parameters = new NeatEvolutionAlgorithmParameters
            {
                ElitismProportion = settings.ElitismProportion,
                SpecieCount = settings.SpeciesCount
            };

            var distanceMetric = new ManhattanDistanceMetric(1.0, 0.0, 10.0);
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 8 };
            var speciationStrategy = new ParallelKMeansClusteringStrategy<NeatGenome>(distanceMetric, parallelOptions);
            var complexityRegulationStrategy = new DefaultComplexityRegulationStrategy(ComplexityCeilingType.Absolute, settings.ComplexityThreshold);

            var evolutionaryAlgorithm = new NeatEvolutionAlgorithm<NeatGenome>(
                parameters,
                speciationStrategy,
                complexityRegulationStrategy);

            var phenomeEvaluator = new RaceSimulationEvaluator(
                simulationStep,
                perceptionStep,
                maximumSimulationTime,
                world,
                inputSamplesCount,
                maximumScanningDistance,
                numberOfSimulationsPerEvaluation: 3);

            var genomeDecoder = new NeatGenomeDecoder(NetworkActivationScheme.CreateAcyclicScheme());
            var genomeListEvaluator = new ParallelGenomeListEvaluator<NeatGenome, IBlackBox>(
                genomeDecoder,
                phenomeEvaluator);

            evolutionaryAlgorithm.Initialize(
                genomeListEvaluator,
                genomeFactory: new NeatGenomeFactory(inputNeuronCount: inputSamplesCount, outputNeuronCount: 2, new NeatGenomeParameters { FeedforwardOnly = true }),
                settings.PopulationSize);

            evolutionaryAlgorithm.UpdateEvent += onUpdate;
            evolutionaryAlgorithm.StartContinue();

            Console.WriteLine("Press enter to stop the evolution.");
            Console.ReadLine();
            Console.WriteLine("Finishing the evolution.");

            evolutionaryAlgorithm.Stop();
            Console.WriteLine("Evolution is stopped.");

            // simulate best individual
            Console.WriteLine("Simulating best individual...");
            evaluate(evolutionaryAlgorithm.CurrentChampGenome);
            Console.WriteLine("Done.");

            void onUpdate(object sender, EventArgs e)
            {
                Console.WriteLine($"Generation #{evolutionaryAlgorithm.CurrentGeneration}");
                Console.WriteLine($"- max fitness:  {evolutionaryAlgorithm.Statistics._maxFitness}");
                Console.WriteLine($"- mean fitness: {evolutionaryAlgorithm.Statistics._meanFitness}");
                Console.WriteLine();

                if (evolutionaryAlgorithm.CurrentGeneration % 10 == 0)
                {
                    Console.WriteLine("Simulating currently best individual...");
                    evaluate(evolutionaryAlgorithm.CurrentChampGenome);
                }
            }

            void evaluate(NeatGenome genome)
            {
                var bestIndividual = genomeDecoder.Decode(genome);
                var lidar = new Lidar(world.Track, inputSamplesCount, maximumScanningDistance);
                var agent = new NeuralNetworkAgent(lidar, bestIndividual);
                var simulation = new Simulation.Simulation(agent, world);
                var summary = simulation.Simulate(simulationStep, perceptionStep, maximumSimulationTime);

                IO.Simulation.StoreResult(world.Track, world.VehicleModel, summary, "", "C:/Users/simon/Projects/racer-experiment/simulator/src/report.json");
            }
        }
    }
}
