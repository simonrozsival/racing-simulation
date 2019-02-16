using CommandLine;
using Racing.Agents;
using Racing.IO;
using Racing.Mathematics;
using Racing.Model;
using Racing.Model.CollisionDetection;
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

            var agent = new RandomAgent(random);
            var vehicleModel = new InaccuratelyMeasuredVehicleModel(
                width: 30,
                length: 60,
                minVelocity: 0,
                maxVelocity: 300,
                minSteeringAngle: -Angle.FromDegrees(45),
                maxSteeringAngle: Angle.FromDegrees(45),
                acceleration: 200,
                steeringAcceleration: Angle.FromDegrees(90),
                bias: 0.05,
                random: random);
            var kineticModel = new KineticModel(vehicleModel);
            // var motionModel = new UnpredictableMotionModel(kineticModel, 0.08, random);
            var motionModel = kineticModel;
            var track = TrackLoader.Load(options.Input);
            var collisionDetector = new AccurateCollisionDetector(track, vehicleModel);
            var goal = new RadialGoal(track.Circuit.WayPoints.Last(), vehicleModel.Length); // todo: I must figure how how to include the "waypoints" into the goal - using "track.Circuit.Goal" now finishes the race in the start position...
            var stateClassificator = new StateClassificator(collisionDetector, goal);

            var results = new List<SimulationResult>(options.NumberOfRepetitions);
            for (var i = 0; i < options.NumberOfRepetitions; i++)
            {
                var result = simulate(agent, track, stateClassificator, motionModel);
                results.Add(result);
            }

            var reachedGoal = results.Where(result => result.Succeeded).ToList();
            var reachingGoalPercentage = (double)reachedGoal.Count / options.NumberOfRepetitions * 100;

            Console.WriteLine($"Agent reached goal {reachedGoal.Count} out of {options.NumberOfRepetitions} times ({reachingGoalPercentage}%).");

            if (reachedGoal.Count > 0)
            {
                var timeToGoal = reachedGoal.Average(result => result.SimulationTime.TotalSeconds);
                var bestTime = reachedGoal.Min(result => result.SimulationTime.TotalSeconds);

                Console.WriteLine($"Average time to goal: {timeToGoal}s");
                Console.WriteLine($"Best time to goal: {bestTime}s");
            }
        }

        private static SimulationResult simulate(IAgent agent, ITrack track, IStateClassificator stateClassificator, IMotionModel motionModel)
        {
            var simulation = new Simulation(agent, track, stateClassificator, motionModel);

            var timeToGoalTask = simulation.Simulate(
                simulationStep: TimeSpan.FromSeconds(1 / 60.0),
                perceptionStep: TimeSpan.FromSeconds(1 / 10.0));

            timeToGoalTask.Wait();

            return timeToGoalTask.Result;
        }
    }
}
