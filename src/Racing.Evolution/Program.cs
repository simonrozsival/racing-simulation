﻿using Racing.IO;
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
using SharpNeat.Network;
using SharpNeat.Phenomes;
using SharpNeat.SpeciationStrategies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Racing.Evolution
{
    class Program
    {
        private const string assetsPath = "../../../../../assets";

        public static void Main(string[] args)
        {
            var random = new Random();
            var circuits = circuitsFilePaths().ToArray();

            var perceptionStep = TimeSpan.FromSeconds(0.1);
            var simulationStep = TimeSpan.FromSeconds(0.05); // 20Hz
            var maximumSimulationTime = TimeSpan.FromSeconds(60);

            var tracks = circuits.Select(circuitPath => Track.Load($"{circuitPath}/circuit_definition.json"));
            var worlds = tracks.Select(track => new StandardWorld(track, simulationStep)).ToArray();

            var inputSamplesCount = 3;
            var maximumScanningDistance = 200;
            ILidar createLidarFor(ITrack track)
                => new Lidar(track, inputSamplesCount, Angle.FromDegrees(135), maximumScanningDistance);

            var settings = new EvolutionSettings
            {
                PopulationSize = 1000,
                SpeciesCount = 30,
                ElitismProportion = 0,
                ComplexityThreshold = 50
            };

            // prepare simulation
            var parameters = new NeatEvolutionAlgorithmParameters   
            {
                ElitismProportion = settings.ElitismProportion,
                SpecieCount = settings.SpeciesCount
            };

            var distanceMetric = new ManhattanDistanceMetric(1.0, 0.0, 10.0);
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 4 };
            var speciationStrategy = new ParallelKMeansClusteringStrategy<NeatGenome>(distanceMetric, parallelOptions);
            var complexityRegulationStrategy = new DefaultComplexityRegulationStrategy(ComplexityCeilingType.Absolute, settings.ComplexityThreshold);

            var evolutionaryAlgorithm = new NeatEvolutionAlgorithm<NeatGenome>(
                parameters,
                speciationStrategy,
                complexityRegulationStrategy);

            var phenomeEvaluator = new RaceSimulationEvaluator(
                random,
                simulationStep,
                perceptionStep,
                maximumSimulationTime,
                worlds,
                createLidarFor);

            var genomeDecoder = new NeatGenomeDecoder(NetworkActivationScheme.CreateAcyclicScheme());
            var genomeListEvaluator = new ParallelGenomeListEvaluator<NeatGenome, IBlackBox>(
                genomeDecoder,
                phenomeEvaluator);

            evolutionaryAlgorithm.Initialize(
                genomeListEvaluator,
                genomeFactory: new NeatGenomeFactory(
                    inputNeuronCount: inputSamplesCount,
                    outputNeuronCount: 2,
                    DefaultActivationFunctionLibrary.CreateLibraryNeat(new BipolarSigmoid()),
                    new NeatGenomeParameters
                    {
                        FeedforwardOnly = true,
                        AddNodeMutationProbability = 0.03,
                        DeleteConnectionMutationProbability = 0.05,
                        ConnectionWeightMutationProbability = 0.08,
                        FitnessHistoryLength = 10,
                    }),
                settings.PopulationSize);

            var lastVisualization = DateTimeOffset.Now;
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

                if (DateTimeOffset.Now - lastVisualization > TimeSpan.FromSeconds(35))
                {
                    lastVisualization = DateTimeOffset.Now;
                    Console.WriteLine("Simulating currently best individual...");
                    evaluate(evolutionaryAlgorithm.CurrentChampGenome);
                }
            }

            void evaluate(NeatGenome genome)
            {
                var worldId = random.Next(0, worlds.Length - 1);
                var world = worlds[worldId];

                var bestIndividual = genomeDecoder.Decode(genome);
                var agent = new NeuralNetworkAgent(createLidarFor(world.Track), bestIndividual);
                var simulation = new Simulation.Simulation(agent, world);
                var summary = simulation.Simulate(simulationStep, perceptionStep, maximumSimulationTime);

                File.Copy($"{circuits[worldId]}/visualization.svg", "C:/Users/simon/Projects/racer-experiment/simulator/src/visualization.svg", overwrite: true);
                IO.Simulation.StoreResult(world.Track, world.VehicleModel, summary, "", "C:/Users/simon/Projects/racer-experiment/simulator/src/report.json");
            }
        }

        private static IEnumerable<string> circuitsFilePaths()
            => new[]
            {
                "generated-at-1550822778155",
                "generated-at-1551107001568",
                "simple-circuit"
            }.Select(circuitName => Path.GetFullPath($"{assetsPath}/tracks/{circuitName}"));
    }
}
