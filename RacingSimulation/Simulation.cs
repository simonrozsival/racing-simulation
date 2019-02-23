using Racing.Agents;
using Racing.Mathematics;
using Racing.Model;
using Racing.Model.Simulation;
using Racing.Model.Vehicle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Racing.Simulation
{
    internal sealed class Simulation : ISimulation
    {
        private readonly IAgent agent;
        private readonly ITrack track;
        private readonly IStateClassificator stateClassificator;
        private readonly IMotionModel motionModel;
        private readonly Log log;

        public IObservable<IEvent> Events { get; }

        public Simulation(
            IAgent agent,
            ITrack track,
            IStateClassificator stateClassificator,
            IMotionModel motionModel)
        {
            this.agent = agent;
            this.track = track;
            this.stateClassificator = stateClassificator;
            this.motionModel = motionModel;
            
            
            log = new Log();

            Events = log.Events;
        }

        public ISummary Simulate(TimeSpan simulationStep, TimeSpan perceptionPeriod, TimeSpan maximumSimulationTime)
        {
            IState vehicleState = new InitialState(track.Circuit);
            IAction nextAction = new NoOpAction();

            var wayPointsQueue = new Queue<IGoal>(track.Circuit.WayPoints);

            var elapsedTime = TimeSpan.Zero;
            var timeToNextPerception = TimeSpan.Zero;

            while (wayPointsQueue.Count > 0 && elapsedTime < maximumSimulationTime)
            {
                timeToNextPerception -= simulationStep;
                if (timeToNextPerception < TimeSpan.Zero)
                {
                    var stopwatch = new System.Diagnostics.Stopwatch();
                    stopwatch.Restart();
                    nextAction = agent.ReactTo(vehicleState);
                    stopwatch.Stop();
                    Console.WriteLine($"Agent was thinking for: {stopwatch.ElapsedMilliseconds}ms");
                    timeToNextPerception = perceptionPeriod;

                    log.ActionSelected(nextAction);
                }

                var predictedStates = motionModel.CalculateNextState(vehicleState, nextAction, simulationStep).ToList();
                vehicleState = predictedStates.Last().state;
                var reachedGoal = false;
                var collided = false;

                foreach (var (time, state) in predictedStates)
                {
                    elapsedTime += time;
                    log.SimulationTimeChanged(elapsedTime);
                    log.StateUpdated(vehicleState);

                    var type = stateClassificator.Classify(state);
                    if (type == StateType.Collision)
                    {
                        elapsedTime = time;
                        vehicleState = state;
                        reachedGoal = false;
                        collided = true;
                        break;
                    }

                    if (type == StateType.Goal)
                    {
                        reachedGoal = true;
                    }
                }

                if (collided)
                {
                    break;
                }

                if (reachedGoal)
                {
                    Console.WriteLine($"Reached next way point, {wayPointsQueue.Count} to go.");
                    wayPointsQueue.Dequeue();
                }
            }

            var timeouted = elapsedTime >= maximumSimulationTime;
            var succeeded = stateClassificator.Classify(vehicleState) == StateType.Goal;
            var result = timeouted ? Result.TimeOut : (succeeded ? Result.Suceeded : Result.Failed);
            log.Finished(result);

            return new SimulationSummary(elapsedTime, result, log.History);
        }

        private sealed class InitialState : IState
        {
            public Vector Position { get; }
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

        private sealed class NoOpAction : IAction
        {
            public double Throttle => 0;
            public double Steering => 0;
        }
    }
}
