using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Racing.IO;
using Racing.Mathematics;
using Racing.Model;
using Racing.Model.Sensing;
using Racing.Model.Simulation;
using Racing.Planning.Domain;
using Racing.Simulation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Racing.Planning
{
    class Program
    {
        public static void Main(string[] args)
        {
            var circuitName = "simple-circuit";
            var circuitPath = Path.GetFullPath($"../../../../../assets/tracks/{circuitName}");

            var perceptionPeriod = TimeSpan.FromSeconds(0.4);
            var simulationStep = perceptionPeriod / 8;

            var track = Track.Load($"{circuitPath}/circuit_definition.json");
            var numberOfLaps = 2;
            var singleLapWayPoints = track.Circuit.WayPoints.ToList();
            // var wayPoints = Enumerable.Range(0, numberOfLaps).SelectMany(_ => singleLapWayPoints).ToList().AsReadOnly();
            var wayPoints = singleLapWayPoints.Take(2).ToList().AsReadOnly();
            var world = new StandardWorld(track, simulationStep);

            //var planner = new HybridAStarPlanner(perceptionPeriod, world, wayPoints);

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

            var planner = new RRTPlanner(
                goalBias: 0.05,
                maximumNumberOfIterations: 20000,
                world,
                new Random(),
                perceptionPeriod,
                wayPoints.Last());

            var exploredStates = new List<IState>();
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

            // store the result in a file so it can be replayed
            var summary = simulate(plan, world);
            //File.Copy($"{circuitPath}/visualization.svg", "C:/Users/simon/Projects/racer-experiment/simulator/src/visualization.svg", overwrite: true);
            IO.Simulation.StoreResult(track, world.VehicleModel, summary, $"{circuitPath}/visualization.svg", "C:/Users/simon/Projects/racer-experiment/simulator/src/report.json");
            Console.WriteLine($"Time to finish: {plan.TimeToGoal.TotalSeconds}s");
        }

        private static ISummary simulate(IPlan plan, IWorldDefinition world)
        {
            var log = new Log();
            var distance = 0.0;

            for (int i = 0; i < plan.Trajectory.Count; i++)
            {
                var trajectory = plan.Trajectory[i];
                var state = trajectory.State;
                var action = trajectory.Action;

                log.SimulationTimeChanged(trajectory.Time);

                if (action != null)
                {
                    log.ActionSelected(action);

                    var nextTrajectorySegmentTime = i < plan.Trajectory.Count - 1
                        ? plan.Trajectory[i + 1].Time
                        : plan.TimeToGoal;
                    var timeStep = nextTrajectorySegmentTime - trajectory.Time;

                    var predictions = world.MotionModel.CalculateNextState(state, action, timeStep);
                    var previousState = trajectory.State;
                    foreach (var (time, predictedState) in predictions)
                    {
                        log.SimulationTimeChanged(trajectory.Time + time);
                        log.StateUpdated(predictedState);

                        distance += Distance.Between(previousState.Position, predictedState.Position);
                        previousState = predictedState;
                    }
                }
            }

            return new SimulationSummary(plan.TimeToGoal, Result.Suceeded, log.History, distance);
        }
    }
}
