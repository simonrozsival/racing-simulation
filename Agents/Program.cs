using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Racing.Agents.Algorithms.Planning;
using Racing.IO;
using Racing.Mathematics;
using Racing.Model;
using Racing.Model.CollisionDetection;
using Racing.Model.Simulation;
using Racing.Model.Vehicle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Racing.Agents
{
    class Program
    {
        public static void Main(string[] args)
        {
            var perceptionPeriod = TimeSpan.FromSeconds(0.4);
            var simulationStep = perceptionPeriod / 4;

            var track = Track.Load("../../../../tracks/simple-circuit/circuit_definition.json");

            var assumedVehicleModel =
                new ForwardDrivingOnlyVehicle(track.Circuit.Radius / 3);
            var realVehicleModel = assumedVehicleModel;

            var collisionDetector = new AccurateCollisionDetector(track, realVehicleModel, safetyMargin: realVehicleModel.Width * 0.5);
            //var collisionDetector = new BoundingSphereCollisionDetector(track, realVehicleModel);

            var assumedMotionModel = new DynamicModel(assumedVehicleModel, collisionDetector, simulationStep);
            var realMotionModel = assumedMotionModel;

            var wayPoints = track.Circuit.WayPoints.ToList().AsReadOnly();
            var stateClassificator = new StateClassificator(collisionDetector, wayPoints.Last());

            var initialState = new InitialState(track.Circuit) as IState;
            var actions = new SteeringInputs(throttleSteps: 5, steeringSteps: 15);

            //var planner = new HybridAStarPlanner(
            //    perceptionPeriod,
            //    realVehicleModel,
            //    realMotionModel,
            //    track,
            //    actions,
            //    wayPoints);

            var planner = new RRTPlanner(
                goalBias: 0.15,
                maximumNumberOfIterations: 100000,
                realVehicleModel,
                realMotionModel,
                track,
                new Random(),
                perceptionPeriod,
                actions,
                wayPoints);

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

            planner.ExploredStates.Subscribe(exploredStates.Add);

            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Restart();
            var plan = planner.FindOptimalPlanFor(initialState);
            stopwatch.Stop();

            if (plan == null)
            {
                Console.WriteLine("Couldn't find any plan.");
                Console.WriteLine(exploredStates.Count);
                flush();
                return;
            }

            Console.WriteLine($"Found a plan in {stopwatch.ElapsedMilliseconds}ms");

            var log = new Log();
            log.StateUpdated(initialState);
            var state = initialState;

            for (int i = 0; i < plan.Trajectory.Count; i++)
            {
                var trajectory = plan.Trajectory[i];
                log.SimulationTimeChanged(trajectory.Time);
                if (trajectory.Action != null)
                {
                    log.ActionSelected(trajectory.Action);
                }
                log.StateUpdated(trajectory.State);
            }

            var summary = new SimulationSummary(plan.TimeToGoal, Result.TimeOut, log.History);
            Simulation.StoreResult(track, realVehicleModel, summary, "C:/Users/simon/Projects/racer-experiment/simulator/src/report.json");
            Console.WriteLine($"Time to finish: {plan.TimeToGoal.TotalSeconds}s");

            flush();
        }

        private sealed class InitialState : IState
        {
            public Point Position { get; }
            public Angle HeadingAngle { get; }
            public Angle SteeringAngle => 0;
            public double Speed => 0;

            public InitialState(ICircuit circuit)
            {
                var startDirection = 
                    circuit.WayPoints.Skip(1).First().Position - circuit.WayPoints.Last().Position;

                Position = circuit.Start;
                HeadingAngle = startDirection.Direction();
            }
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
