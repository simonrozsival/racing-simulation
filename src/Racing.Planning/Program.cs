using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Racing.IO;
using Racing.Mathematics;
using Racing.Model;
using Racing.Model.Planning;
using Racing.Model.Simulation;
using Racing.Model.Vehicle;
using Racing.Simulation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace Racing.Planning
{
    class Program
    {
        private const string assetsDirectoryPath = "../../../../../assets";

        public static void Main(string[] args)
        {
            var circuitName = "simple-circuit";
            var circuitPath = Path.GetFullPath($"{assetsDirectoryPath}/tracks/{circuitName}");

            var perceptionPeriod = TimeSpan.FromSeconds(0.4);
            var simulationStep = perceptionPeriod / 8;

            var track = Track.Load($"{circuitPath}/circuit_definition.json");
            var world = new StandardWorld(track, simulationStep);

            var planner = new HybridAStarPlanner(perceptionPeriod, world, world.WayPoints);

            //var planner = new WayPointFollowingRRTPlannerRRTPlanner(
            //    goalBias: 0.3,
            //    maximumNumberOfIterations: 100000,
            //    realVehicleModel,
            //    realMotionModel,
            //    track,
            //    new Random(),
            //    perceptionPeriod,
            //    actions,
            //    wayPoints);

            //var planner = new RRTPlanner(
            //    goalBias: 0.05,
            //    maximumNumberOfIterations: 20000,
            //    world,
            //    new Random(),
            //    perceptionPeriod,
            //    wayPoints.Last());

            var exploredStates = new List<VehicleState>();
            var lastFlush = DateTimeOffset.Now;
            void flush()
            {
                Console.WriteLine($"Explored {exploredStates.Count} states");
                lastFlush = DateTimeOffset.Now;
                var data = JsonConvert.SerializeObject(exploredStates, new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
                File.WriteAllText("C:/Users/simon/Projects/racer-experiment/simulator/src/progress.json", data);
            }

            flush();

            planner.ExploredStates.Subscribe(state =>
            {
                exploredStates.Add(state);
                if (DateTimeOffset.Now - lastFlush > TimeSpan.FromSeconds(10))
                {
                    flush();
                }
            });

            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Restart();

            var plan = planner.FindOptimalPlanFor(world.InitialState);
            stopwatch.Stop();

            flush();

            if (plan == null)
            {
                Console.WriteLine("Couldn't find any plan.");
                return;
            }

            Console.WriteLine($"Found a plan in {stopwatch.ElapsedMilliseconds}ms");

            var detailedPlan = plan.ToDetailedPlan(world);

            Plans.Store(detailedPlan, $"{assetsDirectoryPath}/plans/plan-{planner.GetType().Name}-{DateTimeOffset.Now.ToUnixTimeSeconds()}-{detailedPlan.TimeToGoal.TotalMilliseconds}.json");

            // store the result in a file so it can be replayed
            var summary = log(detailedPlan);
            //File.Copy($"{circuitPath}/visualization.svg", "C:/Users/simon/Projects/racer-experiment/simulator/src/visualization.svg", overwrite: true);
            IO.Simulation.StoreResult(track, world.VehicleModel, summary, $"{circuitPath}/visualization.svg", "C:/Users/simon/Projects/racer-experiment/simulator/src/report.json");
            Console.WriteLine($"Time to finish: {detailedPlan.TimeToGoal.TotalSeconds}s");
        }

        private static ISummary log(IPlan plan)
        {
            var log = new Log();
            var previousState = plan.Trajectory.First().State;
            var distance = 0.0;

            foreach (var trajectory in plan.Trajectory)
            {
                var state = trajectory.State;
                var action = trajectory.Action;
                log.SimulationTimeChanged(trajectory.Time);

                if (action != null)
                {
                    log.ActionSelected(action);
                }

                log.StateUpdated(state);

                distance += Distance.Between(previousState.Position, state.Position);

                previousState = state;
            }

            return new SimulationSummary(plan.TimeToGoal, Result.Suceeded, log.History, distance);
        }
    }
}
