using Racing.Planning.Algorithms.HybridAStar;
using Racing.Model;
using Racing.Model.CollisionDetection;
using Racing.Model.Vehicle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Racing.Planning.Domain;
using Racing.Planning.Algorithms.HybridAStar.DataStructures;
using Racing.Planning.Algorithms.HybridAStar.Heuristics;

namespace Racing.Planning.Algorithms.Domain
{
    internal class HybridAStarPlanner : IPlanner
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
        private readonly ISubject<IState> exploredStates = new Subject<IState>();

        public IObservable<IState> ExploredStates { get; }

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
                headingAngleCellSize: 2 * Math.PI / 12);

            ExploredStates = exploredStates;
        }

        public IPlan? FindOptimalPlanFor(IState initialState)
        {
            var heuristic = createShortestPathHeuristic(initialState);
            // var heuristic = new EuclideanDistanceHeuristic(vehicleModel.MaxSpeed, problem.Goal);
            // var heuristic = new DijkstraAkaNoHeuristic();

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
                    wayPoints));

            while (!open.IsEmpty)
            {
                var expandedNode = open.DequeueMostPromissing();

                if (expandedNode.RemainingWayPoints.Count == 0)
                {
                    return expandedNode.ReconstructPlan();
                }

                closed.Add(expandedNode.Key);
                exploredStates.OnNext(expandedNode.State);

                expand(expandedNode, open, closed, heuristic, out var nodeReachingGoal);

                if (greedy && nodeReachingGoal != null)
                {
                    return nodeReachingGoal.ReconstructPlan();
                }
            }

            return null;
        }

        private void expand(
            SearchNode node,
            IOpenSet<DiscreteState, SearchNode> open,
            ClosedSet<DiscreteState> closed,
            IHeuristic heuristic,
            out SearchNode? nodeReachingGoal)
        {
            nodeReachingGoal = null;

            foreach (var action in actions.AllPossibleActions)
            {
                var predictedStates = motionModel.CalculateNextState(node.State, action, timeStep).ToList();
                var elapsedTime = predictedStates.Last().relativeTime;
                var nextVehicleState = predictedStates.Last().state;
                var reachedGoal = false;
                var collided = false;

                foreach (var (simulationTime, state) in predictedStates)
                {
                    if (collisionDetector.IsCollision(state))
                    {
                        elapsedTime = simulationTime;
                        nextVehicleState = state;
                        reachedGoal = false;
                        collided = true;
                        break;
                    }

                    if (node.RemainingWayPoints[0].ReachedGoal(state.Position))
                    {
                        reachedGoal = true;
                    }
                }

                var remainingWayPoints = reachedGoal
                    ? node.RemainingWayPoints.Skip(1).ToList()
                    : node.RemainingWayPoints;

                var nextDiscreteState = discretizer.Discretize(nextVehicleState, remainingWayPoints.Count);
                if (closed.Contains(nextDiscreteState))
                {
                    continue;
                }

                if (collided)
                {
                    closed.Add(nextDiscreteState);
                    continue;
                }

                int targetWayPoint = wayPoints.Count - remainingWayPoints.Count;
                var costToCome = node.CostToCome + timeStep.TotalSeconds;
                var costToGo = remainingWayPoints.Count > 0
                    ? heuristic.EstimateTimeToGoal(nextVehicleState, targetWayPoint).TotalSeconds
                    : 0;

                var discoveredNode = new SearchNode(
                    discreteState: nextDiscreteState,
                    state: nextVehicleState,
                    actionFromPreviousState: action,
                    previousState: node,
                    costToCome: costToCome,
                    estimatedTotalCost: costToCome + costToGo,
                    remainingWayPoints);

                if (remainingWayPoints.Count == 0)
                {
                    nodeReachingGoal = discoveredNode;
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

        private GridShortestPathHeuristic createShortestPathHeuristic(IState initialState)
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