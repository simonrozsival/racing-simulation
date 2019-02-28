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
    public sealed class Simulation : ISimulation
    {
        private readonly IAgent agent;
        private readonly IWorldDefinition world;
        private readonly Log log;

        public IObservable<IEvent> Events { get; }

        public Simulation(IAgent agent, IWorldDefinition world)
        {
            this.agent = agent;
            this.world = world;
            
            log = new Log();
            Events = log.Events;
        }

        public ISummary Simulate(TimeSpan simulationStep, TimeSpan perceptionPeriod, TimeSpan maximumSimulationTime)
        {
            var vehicleState = world.InitialState;
            var nextAction = world.Actions.Brake;

            var nextWayPoint = 0;
            var wayPoints = world.WayPoints;

            var elapsedTime = TimeSpan.Zero;
            var timeToNextPerception = TimeSpan.Zero;

            var visualizationSubscription = agent.Visualization.Subscribe(log.Visualize);

            while (nextWayPoint < wayPoints.Count && elapsedTime < maximumSimulationTime)
            {
                timeToNextPerception -= simulationStep;
                if (timeToNextPerception < TimeSpan.Zero)
                {
                    nextAction = agent.ReactTo(vehicleState, nextWayPoint);
                    timeToNextPerception = perceptionPeriod;
                    log.ActionSelected(nextAction);
                }

                var predictedStates = world.MotionModel.CalculateNextState(vehicleState, nextAction, simulationStep).ToList();
                vehicleState = predictedStates.Last().state;
                var reachedGoal = false;
                var collided = false;

                foreach (var (time, state) in predictedStates)
                {
                    elapsedTime += time;
                    log.SimulationTimeChanged(elapsedTime);
                    log.StateUpdated(vehicleState);

                    var type = world.StateClassificator.Classify(state);
                    if (type == StateType.Collision)
                    {
                        elapsedTime = time;
                        vehicleState = state;
                        reachedGoal = false;
                        collided = true;
                        break;
                    }

                    if (wayPoints[nextWayPoint].ReachedGoal(vehicleState.Position))
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
                    nextWayPoint++;
                    // Console.WriteLine($"Reached next way point, {wayPoints.Count - nextWayPoint} to go.");
                }
            }

            var timeouted = elapsedTime >= maximumSimulationTime;
            var succeeded = world.StateClassificator.Classify(vehicleState) == StateType.Goal;
            var result = timeouted ? Result.TimeOut : (succeeded ? Result.Suceeded : Result.Failed);
            log.Finished(result);

            var distanceTravelled = travelledDistance(vehicleState, wayPoints, nextWayPoint);

            return new SimulationSummary(elapsedTime, result, log.History, distanceTravelled);
        }

        private double travelledDistance(IState vehicleState, IReadOnlyList<IGoal> wayPoints, int lastTargetWayPoint)
        {
            if (lastTargetWayPoint == wayPoints.Count)
            {
                return 1.0;
            }

            var distanceBetweenWayPoints = Distance.Between(
                wayPoints[lastTargetWayPoint].Position,
                lastTargetWayPoint > 0
                    ? wayPoints[lastTargetWayPoint - 1].Position
                    : world.Track.Circuit.Start);

            var distanceToTheWayPoint = Distance.Between(wayPoints[lastTargetWayPoint].Position, vehicleState.Position);

            var distanceProportion = distanceToTheWayPoint / distanceBetweenWayPoints;
            var distanceTravelledBetweenWayPoints = Math.Clamp(1 - distanceProportion, -1.0, 1.0);

            return (double)lastTargetWayPoint / wayPoints.Count + distanceTravelledBetweenWayPoints / wayPoints.Count;
        }
    }
}
