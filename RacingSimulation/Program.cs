using CommandLine;
using Racing.Agents;
using Racing.IO;
using Racing.Mathematics;
using Racing.Model;
using Racing.Model.CollisionDetection;
using Racing.Model.Simulation;
using Racing.Model.Vehicle;
using Racing.Simulation.Vehicle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Racing.Simulation
{
    class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(run);
        }

        private static void run(Options options)
        {
            var random = new Random(Seed: 123);
            var perceptionPeriod = TimeSpan.FromSeconds(0.8);
            var simulationStep = TimeSpan.FromSeconds(1 / 60.0);

            var track = Track.Load(options.Input);

            var assumedVehicleModel =
                VehicleModelFactory.ForwardDrivingOnlyWhichFitsOnto(track);
            var assumedMotionModel = new KineticModel(assumedVehicleModel);

            //var realVehicleModel = new InaccuratelyMeasuredVehicleModel(
            //    assumedVehicleModel,
            //    bias: 0.05,
            //    random: random);
            //var realMotionModel = new UnpredictableMotionModel(assumedMotionModel, 0.08, random);

            var realVehicleModel = assumedVehicleModel;
            var realMotionModel = assumedMotionModel;

            var collisionDetector = new AccurateCollisionDetector(track, realVehicleModel);
            // todo: I must figure how how to include the "waypoints" into the goal - using "track.Circuit.Goal" now finishes the race in the start position...
            var goal = new RadialGoal(track.Circuit.WayPoints.Last(), realVehicleModel.Length);
            var stateClassificator = new StateClassificator(collisionDetector, goal);

            var results = new List<ISummary>(options.NumberOfRepetitions);
            for (var i = 0; i < options.NumberOfRepetitions; i++)
            {
                Console.WriteLine($"Simulaton run #{i}...");
                var agent = new AStarAgent(assumedVehicleModel, assumedMotionModel, track, perceptionPeriod);
                var simulation = new Simulation(agent, track, stateClassificator, realMotionModel);

                var summary = simulation.Simulate(
                    simulationStep,
                    perceptionPeriod,
                    maximumSimulationTime: TimeSpan.FromMinutes(1));

                Console.WriteLine($"Result: {summary.Result.ToString()} after {summary.SimulationTime.TotalSeconds}s");

                var fileName = $"{options.Output}/run-{i}.json";
                IO.Simulation.StoreResult(track, realVehicleModel, summary, fileName);
                Console.WriteLine($"Storing  result into: {fileName}");
                Console.WriteLine($"=====================");

                results.Add(summary);
            }

            evaluate(results);
        }

        private static void evaluate(IList<ISummary> results)
        {
            var reachedGoal = results.Where(result => result.Result == Result.Suceeded).ToList();
            var reachingGoalPercentage = (double)reachedGoal.Count / results.Count * 100;

            Console.WriteLine($"Agent reached goal {reachedGoal.Count} out of {results.Count} times ({reachingGoalPercentage}%).");

            if (reachedGoal.Count > 0)
            {
                var timeToGoal = reachedGoal.Average(result => result.SimulationTime.TotalSeconds);
                var bestTime = reachedGoal.Min(result => result.SimulationTime.TotalSeconds);

                Console.WriteLine($"Average time to goal: {timeToGoal}s");
                Console.WriteLine($"Best time to goal: {bestTime}s");
            }
        }
    }
}
