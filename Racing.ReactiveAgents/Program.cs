using Racing.IO;
using Racing.Model;
using Racing.Model.Actions;
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
            var trajectory = new Trajectory(plan.Trajectory);

            var agent = new DynamcWindowApproachAgent(
                trajectory,
                world,
                4 * world.VehicleModel.Length,
                perceptionPeriod,
                4 * perceptionPeriod);

            //var agent = new PIDAgent(plan.Trajectory, world.MotionModel, perceptionPeriod);

            Console.WriteLine($"Simulating agent {agent.GetType().Name}");
            var simulation = new Simulation.Simulation(agent, world);
            var summary = simulation.Simulate(simulationStep, perceptionPeriod, TimeSpan.FromSeconds(60));

            IO.Simulation.StoreResult(track, world.VehicleModel, summary, $"{circuitPath}/visualization.svg", "C:/Users/simon/Projects/racer-experiment/simulator/src/report.json");
            Console.WriteLine($"Time to finish: {summary.SimulationTime.TotalSeconds}s");
        }
    }
}
