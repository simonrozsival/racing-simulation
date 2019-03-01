using Racing.Planning.Algorithms.HybridAStar;
using Racing.Model;
using Racing.Model.CollisionDetection;
using Racing.Model.Vehicle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Racing.Planning.Algorithms.HybridAStar.DataStructures;
using Racing.Planning.Algorithms.HybridAStar.Heuristics;
using Racing.Model.Planning;

namespace Racing.Planning
{
    public class HybridAStarPlanner : IPlanner
    {
        private readonly TimeSpan timeStep;
        private readonly IVehicleModel vehicleModel;
        private readonly IMotionModel motionModel;
        private readonly ITrack track;
        private readonly IActionSet actions;
        private readonly IReadOnlyList<IGoal> wayPoints;
        private readonly ICollisionDetector collisionDetector;
        private readonly bool greedy;

        private readonly StateDiscretizer discretizer;
        private readonly ISubject<VehicleState> exploredStates = new Subject<VehicleState>();

        public IObservable<VehicleState> ExploredStates { get; }

        public HybridAStarPlanner(
            TimeSpan timeStep,
            IWorldDefinition world,
            IReadOnlyList<IGoal> wayPoints,
            bool greedy = false)
        {
            this.timeStep = timeStep;
            this.greedy = greedy;
            this.wayPoints = wayPoints;

            vehicleModel = world.VehicleModel;
            motionModel = world.MotionModel;
            track = world.Track;
            actions = world.Actions;
            collisionDetector = world.CollisionDetector;

            discretizer = new StateDiscretizer(
                positionXCellSize: vehicleModel.Width / 2,
                positionYCellSize: vehicleModel.Width / 2,
                headingAngleCellSize: 2 * Math.PI / 36);

            ExploredStates = exploredStates;
        }

        public IPlan? FindOptimalPlanFor(VehicleState initialState)
        {
            var heuristic = createShortestPathHeuristic(initialState);

            var open = new BinaryHeapOpenSet<DiscreteState, SearchNode>();
            var closed = new ClosedSet<DiscreteState>();

            open.Add(
                new SearchNode(
                    discreteState: discretizer.Discretize(initialState, wayPoints.Count),
                    state: initialState,
                    actionFromPreviousState: null,
                    previousState: null,
                    costToCome: 0,
                    estimatedTotalCost: 0, // we don't need estimate for the initial node
                    targetWayPoint: 0));

            while (!open.IsEmpty)
            {
                var node = open.DequeueMostPromissing();

                if (node.TargetWayPoint == wayPoints.Count)
                {
                    return node.ReconstructPlan();
                }

                closed.Add(node.Key);
                exploredStates.OnNext(node.State);

                foreach (var action in actions.AllPossibleActions)
                {
                    var predictedStates = motionModel.CalculateNextState(node.State, action, timeStep);
                    var elapsedTime = predictedStates.Last().relativeTime;
                    var nextVehicleState = predictedStates.Last().state;
                    var reachedWayPoint = false;
                    var collided = false;

                    foreach (var (simulationTime, state) in predictedStates)
                    {
                        if (collisionDetector.IsCollision(state))
                        {
                            elapsedTime = simulationTime;
                            nextVehicleState = state;
                            reachedWayPoint = false;
                            collided = true;
                            break;
                        }

                        if (wayPoints[node.TargetWayPoint].ReachedGoal(state.Position))
                        {
                            reachedWayPoint = true;
                        }
                    }

                    var nextTargetWayPoint = reachedWayPoint
                        ? node.TargetWayPoint + 1
                        : node.TargetWayPoint;

                    var nextDiscreteState = discretizer.Discretize(nextVehicleState, nextTargetWayPoint);
                    if (closed.Contains(nextDiscreteState))
                    {
                        continue;
                    }

                    if (collided)
                    {
                        closed.Add(nextDiscreteState);
                        continue;
                    }

                    var costToCome = node.CostToCome + timeStep.TotalSeconds;
                    var reachedLastWayPoint = nextTargetWayPoint != wayPoints.Count;
                    var costToGo = reachedLastWayPoint
                        ? heuristic.EstimateTimeToGoal(nextVehicleState, nextTargetWayPoint).TotalSeconds
                        : 0;

                    var discoveredNode = new SearchNode(
                        discreteState: nextDiscreteState,
                        state: nextVehicleState,
                        actionFromPreviousState: action,
                        previousState: node,
                        costToCome: costToCome,
                        estimatedTotalCost: costToCome + costToGo,
                        nextTargetWayPoint);

                    if (greedy && reachedLastWayPoint)
                    {
                        return discoveredNode.ReconstructPlan();
                    }

                    if (!open.Contains(discoveredNode.Key))
                    {
                        open.Add(discoveredNode);
                    }
                    else if (discoveredNode.CostToCome < open.Get(discoveredNode.Key).CostToCome)
                    {
                        open.ReplaceExistingWithTheSameKey(discoveredNode);
                    }
                }
            }

            return null;
        }

        private GridShortestPathHeuristic createShortestPathHeuristic(VehicleState initialState)
        {
            var heuristic = new GridShortestPathHeuristic(
                initialState.Position,
                wayPoints,
                track,
                new BoundingSphereCollisionDetector(track, vehicleModel),
                stepSize: vehicleModel.Length,
                maxSpeed: vehicleModel.MaxSpeed);

            return heuristic;
        }
    }
}