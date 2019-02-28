using Racing.IO;
using Racing.Model;
using System;
using System.IO;

namespace Racing.ReactiveAgents
{
    class Program
    {
        private const string assetsDirectoryPath = "../../../../assets";

        public static void Main(string[] args)
        {
            var circuitName = "simple-circuit";
            var circuitPath = Path.GetFullPath($"{assetsDirectoryPath}/tracks/{circuitName}");

            var perceptionPeriod = TimeSpan.FromSeconds(1.0 / 10.0);
            var simulationStep = TimeSpan.FromSeconds(1.0 / 20.0);

            Console.WriteLine($"Loading track {circuitName}");
            var track = Track.Load($"{circuitPath}/circuit_definition.json");
            var world = new StandardWorld(track, simulationStep, safetyMargin: 0.5);

            var planName = "plan-HybridAStarPlanner-1551383968-15200.json";
            Console.WriteLine($"Loading plan {planName}");
            var plan = Plans.Load($"{assetsDirectoryPath}/plans/{planName}");

            var agent = new DynamcWindowApproachAgent(
                plan.Trajectory,
                world.VehicleModel,
                world.Track,
                world.CollisionDetector,
                world.MotionModel,
                world.Actions,
                4 * world.VehicleModel.Length,
                perceptionPeriod);

            Console.WriteLine($"Simulating agent {agent.GetType().Name}");
            var simulation = new Simulation.Simulation(agent, world);
            var summary = simulation.Simulate(simulationStep, perceptionPeriod, TimeSpan.FromSeconds(30));

            IO.Simulation.StoreResult(track, world.VehicleModel, summary, $"{circuitPath}/visualization.svg", "C:/Users/simon/Projects/racer-experiment/simulator/src/report.json");
            Console.WriteLine($"Time to finish: {summary.SimulationTime.TotalSeconds}s");
        }
    }
}
