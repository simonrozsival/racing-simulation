using Racing.IO;
using Racing.Model;
using Racing.Model.Simulation;
using Racing.Planning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Racing.ReactiveAgents
{
    class Program
    {
        public static void Main(string[] args)
        {
            var circuitName = "simple-circuit";
            var circuitPath = Path.GetFullPath($"../../../../assets/tracks/{circuitName}");

            var planningPeriod = TimeSpan.FromSeconds(0.4);
            var perceptionPeriod = TimeSpan.FromSeconds(0.1);
            var simulationStep = TimeSpan.FromSeconds(0.016);

            var track = Track.Load($"{circuitPath}/circuit_definition.json");
            var wayPoints = track.Circuit.WayPoints.ToList().AsReadOnly();
            var world = new StandardWorld(track, simulationStep);

            var planner = new HybridAStarPlanner(planningPeriod, world, wayPoints);

            Console.WriteLine("Searching for a plan...");
            var plan = planner.FindOptimalPlanFor(world.InitialState);

            if (plan == null)
            {
                Console.WriteLine("Couldn't find any plan.");
                return;
            }

            Console.WriteLine($"Found a plan.");

            var agent = new DynamcWindowApproachAgent(
                plan.Trajectory.Select(segment => segment.State).ToArray(),
                world.VehicleModel,
                world.Track,
                world.CollisionDetector,
                world.MotionModel,
                world.Actions,
                world.Track.Circuit.Radius,
                perceptionPeriod);

            var simulation = new Simulation.Simulation(agent, world);
            var summary = simulation.Simulate(simulationStep, perceptionPeriod, TimeSpan.FromSeconds(60));

            IO.Simulation.StoreResult(track, world.VehicleModel, summary, $"{circuitPath}/visualization.svg", "C:/Users/simon/Projects/racer-experiment/simulator/src/report.json");
            Console.WriteLine($"Time to finish: {summary.SimulationTime.TotalSeconds}s");
        }

        private sealed class SimulationSummary : ISummary
        {
            public SimulationSummary(TimeSpan simulationTime, Result result, IEnumerable<IEvent> log, double distanceTravelled)
            {
                SimulationTime = simulationTime;
                Result = result;
                Log = log;
                DistanceTravelled = distanceTravelled;
            }

            public TimeSpan SimulationTime { get; }
            public Result Result { get; }
            public IEnumerable<IEvent> Log { get; }
            public double DistanceTravelled { get; }
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
