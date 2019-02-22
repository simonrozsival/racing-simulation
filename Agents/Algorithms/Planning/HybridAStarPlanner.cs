using Racing.Agents.Algorithms.Planning.HybridAStar;
using Racing.Agents.Algorithms.Planning.HybridAStar.DataStructures;
using Racing.Agents.Algorithms.Planning.HybridAStar.Heuristics;
using Racing.Model;
using Racing.Model.CollisionDetection;
using Racing.Model.Vehicle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

namespace Racing.Agents.Algorithms.Planning
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

        private readonly StateDiscretizer discretizer;
        private readonly ISubject<IState> exploredStates = new Subject<IState>();

        public IObservable<IState> ExploredStates { get; }

        public HybridAStarPlanner(
            TimeSpan timeStep,
            IVehicleModel vehicleModel,
            IMotionModel motionModel,
            ITrack track,
            IActionSet actions,
            IReadOnlyList<IGoal> wayPoints,
            ICollisionDetector collisionDetector)
        {
            this.timeStep = timeStep;
            this.vehicleModel = vehicleModel;
            this.motionModel = motionModel;
            this.track = track;
            this.actions = actions;
            this.wayPoints = wayPoints;
            this.collisionDetector = collisionDetector;

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
            var wayPoints = track.Circuit.WayPoints.ToList().AsReadOnly();

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

                foreach (var action in actions.AllPossibleActions)
                {
                    var predictedStates = motionModel.CalculateNextState(expandedNode.State, action, timeStep).ToList();
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

                        if (expandedNode.RemainingWayPoints[0].ReachedGoal(state.Position))
                        {
                            reachedGoal = true;
                        }
                    }

                    var remainingWayPoints = reachedGoal
                        ? expandedNode.RemainingWayPoints.Skip(1).ToList()
                        : expandedNode.RemainingWayPoints;

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

                    // time can be different from one whole "step" because the simulation could have broken at some sub-step
                    // if it reached a goal somewhere in the middle of the time step.
                    var costToCome = expandedNode.CostToCome + timeStep.TotalSeconds;
                    var costToGo = remainingWayPoints.Count > 0
                        ? heuristic.EstimateTimeToGoal(nextVehicleState).TotalSeconds
                        : 0;

                    var discoveredNode = new SearchNode(
                        discreteState: nextDiscreteState,
                        state: nextVehicleState,
                        actionFromPreviousState: action,
                        previousState: expandedNode,
                        costToCome: costToCome,
                        estimatedTotalCost: costToCome + costToGo,
                        remainingWayPoints);

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

        private IHeuristic createShortestPathHeuristic(IState initialState)
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