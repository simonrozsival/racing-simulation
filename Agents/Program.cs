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
            var simulationStep = perceptionPeriod / 8;

            var track = Track.Load("../../../../tracks/simple-circuit/circuit_definition.json");

            var assumedVehicleModel =
                new ForwardDrivingOnlyVehicle(track.Circuit.Radius / 2.5);
            var realVehicleModel = assumedVehicleModel;

            var collisionDetector = new AccurateCollisionDetector(track, realVehicleModel, safetyMargin: realVehicleModel.Width * 0.5);
            //var collisionDetector = new BoundingSphereCollisionDetector(track, realVehicleModel);

            var assumedMotionModel = new DynamicModel(assumedVehicleModel, collisionDetector, simulationStep);
            var realMotionModel = assumedMotionModel;

            var goal = new RadialGoal(track.Circuit.WayPoints.ElementAt(4), realVehicleModel.Length);
            var stateClassificator = new StateClassificator(collisionDetector, goal);

            IState initialState = new InitialState(track.Circuit);

            var planningProblem = new PlanningProblem(initialState, new SteeringInputs(), goal);
            var aStarPlanner = new HybridAStarPlanner(
                collisionDetector,
                perceptionPeriod,
                realVehicleModel,
                realMotionModel,
                track);

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

            aStarPlanner.ExploredStates.Subscribe(exploredStates.Add);

            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Restart();
            var plan = aStarPlanner.FindOptimalPlanFor(planningProblem);
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
            var elapsedTime = TimeSpan.Zero;
            var state = initialState;

            var length = 0.0;

            foreach (var action in plan.Actions)
            {
                log.ActionSelected(action);
                var time = perceptionPeriod;
                while (time > TimeSpan.Zero)
                {
                    var step = time < simulationStep ? time : simulationStep;
                    time -= step;
                    elapsedTime += step;
                    log.SimulationTimeChanged(elapsedTime);

                    var prevPosition = state.Position;
                    state = realMotionModel.CalculateNextState(state, action, step);
                    length += (state.Position - prevPosition).CalculateLength();

                    log.StateUpdated(state);
                }
            }

            var summary = new SimulationSummary(elapsedTime, Result.TimeOut, log.History);
            Simulation.StoreResult(track, realVehicleModel, summary, "C:/Users/simon/Projects/racer-experiment/simulator/src/report.json");

            Console.WriteLine($"Path length: {length / (realVehicleModel.Width / 1.85)}m");
            Console.WriteLine($"Time to finish: {elapsedTime.TotalSeconds}s");

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
                var startDirection = circuit.WayPoints.Skip(1).First() - circuit.WayPoints.Last();

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
