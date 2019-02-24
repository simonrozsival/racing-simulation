using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Racing.Planning.Algorithms.Domain;
using Racing.IO;
using Racing.Mathematics;
using Racing.Model;
using Racing.Model.Sensing;
using Racing.Model.Simulation;
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
            var circuitName = "generated-at-1550822778155";
            var circuitPath = Path.GetFullPath($"../../../../../assets/tracks/{circuitName}");

            var perceptionPeriod = TimeSpan.FromSeconds(0.4);
            var simulationStep = perceptionPeriod / 8;

            var track = Track.Load($"{circuitPath}/circuit_definition.json");
            var wayPoints = track.Circuit.WayPoints.ToList().AsReadOnly();
            var world = new StandardWorld(track, simulationStep);

            var planner = new HybridAStarPlanner(perceptionPeriod, world, wayPoints);

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
            //    goalBias: 0.3,
            //    maximumNumberOfIterations: 100000,
            //    realVehicleModel,
            //    realMotionModel,
            //    track,
            //    collisionDetector,
            //    new Random(),
            //    perceptionPeriod,
            //    actions,
            //    wayPoints.SkipLast(1).Last());

            var exploredStates = new List<IState>();
            var lastFlush = DateTimeOffset.Now;
            void flush()
            {
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

            if (plan == null)
            {
                Console.WriteLine("Couldn't find any plan.");
                Console.WriteLine($"Explored states: {exploredStates.Count}");
                flush();
                return;
            }

            Console.WriteLine($"Found a plan in {stopwatch.ElapsedMilliseconds}ms");

            var log = new Log();
            log.StateUpdated(world.InitialState);
            var state = world.InitialState;

            var lidar = new Lidar(
                track,
                samplingFrequency: 11,
                maximumDistance: Length.FromMeters(160));


            Console.WriteLine("const lidarScans = [");
            for (int i = 0; i < plan.Trajectory.Count; i++)
            {
                var trajectory = plan.Trajectory[i];
                log.SimulationTimeChanged(trajectory.Time);
                if (trajectory.Action != null)
                {
                    log.ActionSelected(trajectory.Action);
                }
                log.StateUpdated(trajectory.State);

                Console.WriteLine("{");
                Console.WriteLine($"  time: {trajectory.Time.TotalSeconds},");
                Console.WriteLine($"  origin: [{trajectory.State.Position.X.Meters}, {trajectory.State.Position.Y.Meters}],");
                Console.WriteLine($"  points: [");

                var reading = lidar.Scan(trajectory.State.Position, trajectory.State.HeadingAngle);
                foreach (var point in reading.ToPointCloud())
                {
                    Console.WriteLine($"    [{point.X.Meters}, {point.Y.Meters}],");
                }

                Console.WriteLine("  ]");
                Console.WriteLine("},");
            }
            Console.WriteLine("];");

            var summary = new SimulationSummary(plan.TimeToGoal, Result.TimeOut, log.History);
            Simulation.StoreResult(track, world.VehicleModel, summary, $"{circuitPath}/visualization.svg", "C:/Users/simon/Projects/racer-experiment/simulator/src/report.json");
            Console.WriteLine($"Time to finish: {plan.TimeToGoal.TotalSeconds}s");

            flush();
        }

        private sealed class SimulationSummary : ISummary
        {
            public SimulationSummary(TimeSpan simulationTime, Result result, IEnumerable<IEvent> log)
            {
                SimulationTime = simulationTime;
                Result = result;
                Log = log;
            }

            public TimeSpan SimulationTime { get; }
            public Result Result { get; }
            public IEnumerable<IEvent> Log { get; }
        }

        public sealed class Log
        {
            private readonly List<IEvent> history = new List<IEvent>();
            private readonly ISubject<IEvent> events = new Subject<IEvent>();

            private TimeSpan simulationTime = TimeSpan.Zero;

            public IEnumerable<IEvent> History => history;
            public IObservable<IEvent> Events => events.AsObservable();

            public void SimulationTimeChanged(TimeSpan time)
            {
                simulationTime = time;
            }

            public void ActionSelected(IAction action)
            {
                log(new ActionSelectedEvent(action, simulationTime));
            }

            public void StateUpdated(IState state)
            {
                log(new StateUpdatedEvent(state, simulationTime));
            }

            public void Finished(Result result)
            {
                log(new SimulationEndedEvent(result, simulationTime));
                events.OnCompleted();
            }

            private void log(IEvent loggedEvent)
            {
                events.OnNext(loggedEvent);
                history.Add(loggedEvent);
            }

            private sealed class ActionSelectedEvent : IActionSelectedEvent
            {
                public ActionSelectedEvent(IAction action, TimeSpan time)
                {
                    Action = action;
                    Time = time;
                }

                public IAction Action { get; }

                public TimeSpan Time { get; }
            }

            private sealed class SimulationEndedEvent : ISimulationEndedEvent
            {
                public SimulationEndedEvent(Result result, TimeSpan time)
                {
                    Result = result;
                    Time = time;
                }

                public Result Result { get; }

                public TimeSpan Time { get; }
            }


            private sealed class StateUpdatedEvent : IStateUpdatedEvent
            {
                public StateUpdatedEvent(IState state, TimeSpan time)
                {
                    State = state;
                    Time = time;
                }

                public IState State { get; }

                public TimeSpan Time { get; }
            }

        }
    }
}
