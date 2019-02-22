using CommandLine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Racing.Agents;
using Racing.IO;
using Racing.Model;
using Racing.Model.CollisionDetection;
using Racing.Model.Simulation;
using Racing.Model.Vehicle;
using Racing.Simulation.Vehicle;
using System;
using System.Collections.Generic;
using System.IO;
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
            var circuitPath = Path.GetFullPath(options.Input);

            var random = new Random(Seed: 123);
            var perceptionPeriod = TimeSpan.FromSeconds(0.4);
            var simulationStep = TimeSpan.FromSeconds(1 / 60.0);

            var track = Track.Load($"{circuitPath}/circuit_definition.json");

            var assumedVehicleModel =
                VehicleModelFactory.ForwardDrivingOnlyWhichFitsOnto(track);
            var realVehicleModel = assumedVehicleModel;
            var collisionDetector = new AccurateCollisionDetector(track, realVehicleModel);

            var assumedMotionModel = new DynamicModel(assumedVehicleModel, collisionDetector, simulationStep);

            //var realVehicleModel = new InaccuratelyMeasuredVehicleModel(
            //    assumedVehicleModel,
            //    bias: 0.05,
            //    random: random);
            //var realMotionModel = new UnpredictableMotionModel(assumedMotionModel, 0.08, random);

            var realMotionModel = assumedMotionModel;

            var goal = track.Circuit.WayPoints.Last();
            var stateClassificator = new StateClassificator(collisionDetector, goal);

            var results = new List<ISummary>(options.NumberOfRepetitions);
            for (var i = 0; i < options.NumberOfRepetitions; i++)
            {
                Console.WriteLine($"Simulaton run #{i}...");
                var actions = new SteeringInputs();
                var agent = new AStarAgent(assumedVehicleModel, assumedMotionModel, track, actions, perceptionPeriod);
                var simulation = new Simulation(agent, track, stateClassificator, realMotionModel);

                var exploredStates = new List<IState>();
                void flush()
                {
                    var data = JsonConvert.SerializeObject(exploredStates, new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });
                    File.WriteAllText("C:/Users/simon/Projects/racer-experiment/simulator/src/progress.json", data);
                }

                agent.ExploredStates.Subscribe(
                    exploredState =>
                    {
                        exploredStates.Add(exploredState);
                        if (exploredStates.Count % 100 == 0)
                        {
                        //flush();
                        //Task.Delay(TimeSpan.FromSeconds(2)).Wait();
                    }
                    });

                var summary = simulation.Simulate(
                    simulationStep,
                    perceptionPeriod,
                    maximumSimulationTime: TimeSpan.FromMinutes(1));

                Console.WriteLine($"Result: {summary.Result.ToString()} after {summary.SimulationTime.TotalSeconds}s");

                // var fileName = $"{options.Output.TrimEnd('/')}/run-{i}.json";
                var fileName = "C:/Users/simon/Projects/racer-experiment/simulator/src/report.json";
                IO.Simulation.StoreResult(track, realVehicleModel, summary, $"{circuitPath}/visualization.svg", fileName);
                Console.WriteLine($"Storing  result into: {fileName}");
                Console.WriteLine($"=====================");

                results.Add(summary);

                flush();
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
