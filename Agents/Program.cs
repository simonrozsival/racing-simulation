using Racing.Agents.Algorithms.Planning;
using Racing.IO;
using Racing.Mathematics;
using Racing.Model;
using Racing.Model.CollisionDetection;
using Racing.Model.Simulation;
using Racing.Model.Vehicle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Racing.Agents
{
    class Program
    {
        public static void Main(string[] args)
        {
            var perceptionPeriod = TimeSpan.FromSeconds(0.8);
            var simulationStep = perceptionPeriod / 20;
            var numberOfSimulationsPerAction = (int)(perceptionPeriod / simulationStep);

            var track = Track.Load("../../../../tracks/simple-circuit/circuit_definition.json");

            var assumedVehicleModel =
                new ForwardDrivingOnlyVehicle(track.Circuit.Radius / 2.5);
            var assumedMotionModel = new KineticModel(assumedVehicleModel);

            var agent = new AStarAgent(assumedVehicleModel, assumedMotionModel, track, perceptionPeriod);

            var realVehicleModel = assumedVehicleModel;
            var realMotionModel = assumedMotionModel;

            var collisionDetector = new AccurateCollisionDetector(track, realVehicleModel);
            var goal = new RadialGoal(track.Circuit.WayPoints.Skip(5).First(), realVehicleModel.Length);
            var stateClassificator = new StateClassificator(collisionDetector, goal);

            IState initialState = new InitialState(track.Circuit);

            var planningProblem = new PlanningProblem(initialState, realVehicleModel, realMotionModel, SteeringAction.PossibleActions, track, goal);
            var aStarPlanner = new AStarPlanner(new BoundingSphereCollisionDetector(track, realVehicleModel), perceptionPeriod, simulationStep);

            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Restart();
            var plan = aStarPlanner.FindOptimalPlanFor(planningProblem);
            stopwatch.Stop();

            if (plan == null)
            {
                Console.WriteLine("Couldn't find any plan.");
                return;
            }

            Console.WriteLine($"Found a plan in {stopwatch.ElapsedMilliseconds}ms");

            var log = new Log();
            log.StateUpdated(initialState);
            var elapsedTime = TimeSpan.Zero;
            var state = initialState;

            foreach (var action in plan)
            {
                log.ActionSelected(action);
                for (var i = 0; i < numberOfSimulationsPerAction; i++)
                {
                    elapsedTime += simulationStep;
                    log.SimulationTimeChanged(elapsedTime);
                    state = realMotionModel.CalculateNextState(state, action, simulationStep);
                    log.StateUpdated(state);
                }
            }

            var summary = new SimulationSummary(elapsedTime, Result.TimeOut, log.History);
            Simulation.StoreResult(track, realVehicleModel, summary, "C:/Users/simon/Projects/racer-experiment/simulator/src/report.json");
        }

        private sealed class InitialState : IState
        {
            public Point Position { get; }
            public Angle HeadingAngle { get; }
            public Angle SteeringAngle => 0;
            public double Velocity => 0;

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
