using Racing.Agents.Algorithms.Planning.HybridAStar;
using Racing.Agents.Algorithms.Planning.HybridAStar.DataStructures;
using Racing.Agents.Algorithms.Planning.HybridAStar.Heuristics;
using Racing.Model;
using Racing.Model.CollisionDetection;
using Racing.Model.Vehicle;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Racing.Agents.Algorithms.Planning
{
    internal class HybridAStarPlanner : IPlanner
    {
        private readonly TimeSpan timeStep;
        private readonly ICollisionDetector collisionDetector;
        private readonly IVehicleModel vehicleModel;
        private readonly IMotionModel motionModel;
        private readonly ITrack track;
        private readonly StateDiscretizer discretizer;

        public ISubject<IState> ExploredStates { get; } = new Subject<IState>();

        public HybridAStarPlanner(
            ICollisionDetector collisionDetector,
            TimeSpan timeStep,
            IVehicleModel vehicleModel,
            IMotionModel motionModel,
            ITrack track)
        {
            this.timeStep = timeStep;
            this.collisionDetector = collisionDetector;
            this.vehicleModel = vehicleModel;
            this.motionModel = motionModel;
            this.track = track;

            discretizer = new StateDiscretizer(
                positionXCellSize: vehicleModel.Width / 2,
                positionYCellSize: vehicleModel.Width / 2,
                headingAngleCellSize: 2 * Math.PI / 12);
        }

        public IPlan? FindOptimalPlanFor(PlanningProblem problem)
        {
            var heuristic = createShortestPathHeuristic(problem);
            // var heuristic = new EuclideanDistanceHeuristic(vehicleModel.MaxSpeed, problem.Goal);
            // var heuristic = new DijkstraAkaNoHeuristic();

            var open = new BinaryHeapOpenSet<DiscreteState, SearchNode>();
            var closed = new ClosedSet<DiscreteState>();

            open.Add(
                new SearchNode(
                    discreteState: discretizer.Discretize(problem.InitialState),
                    state: problem.InitialState,
                    actionFromPreviousState: null,
                    previousState: null,
                    costToCome: 0,
                    estimatedTotalCost: heuristic.EstimateTimeToGoal(problem.InitialState).TotalSeconds));

            while (!open.IsEmpty)
            {
                var expandedNode = open.DequeueMostPromissing();

                if (problem.Goal.ReachedGoal(expandedNode.State.Position))
                {
                    return reconstructPlan(expandedNode);
                }

                closed.Add(expandedNode.Key);
                ExploredStates.OnNext(expandedNode.State);

                foreach (var action in problem.Actions.AllPossibleActions)
                {
                    var nextVehicleState = motionModel.CalculateNextState(expandedNode.State, action, timeStep, problem.Goal);

                    var nextDiscreteState = discretizer.Discretize(nextVehicleState);
                    if (closed.Contains(nextDiscreteState))
                    {
                        continue;
                    }

                    if (collisionDetector.IsCollision(nextVehicleState))
                    {
                        closed.Add(nextDiscreteState);
                        continue;
                    }

                    // time can be different from one whole "step" because the simulation could have broken at some sub-step
                    // if it reached a goal somewhere in the middle of the time step.
                    var costToCome = expandedNode.CostToCome + timeStep.TotalSeconds;
                    var discoveredNode = new SearchNode(
                        discreteState: nextDiscreteState,
                        state: nextVehicleState,
                        actionFromPreviousState: action,
                        previousState: expandedNode,
                        costToCome: costToCome,
                        estimatedTotalCost: costToCome + heuristic.EstimateTimeToGoal(nextVehicleState).TotalSeconds);

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

        private IHeuristic createShortestPathHeuristic(PlanningProblem problem)
        {
            var heuristic = new GridShortestPathHeuristic(
                problem.InitialState.Position,
                problem.Goal,
                track,
                new BoundingSphereCollisionDetector(track, vehicleModel),
                stepSize: vehicleModel.Length,
                maxSpeed: vehicleModel.MaxSpeed);

            return heuristic;
        }

        private IPlan reconstructPlan(SearchNode? node)
        {
            var timeToGoal = node?.CostToCome ?? 0;
            var states = new List<IState>();
            var actions = new List<IAction>();
            while (node != null)
            {
                states.Insert(0, node.State);
                if (node.ActionFromPreviousState != null)
                {
                    actions.Insert(0, node.ActionFromPreviousState);
                }

                node = node.PreviousNode;
            }

            return new Plan(TimeSpan.FromSeconds(timeToGoal), states, actions);
        }

        internal sealed class SearchNode : ISearchNode<DiscreteState>, IComparable<SearchNode>
        {
            public DiscreteState Key { get; }
            public IState State { get; }
            public IAction? ActionFromPreviousState { get; }
            public SearchNode? PreviousNode { get; }
            public double CostToCome { get; }
            public double EstimatedTotalCost { get; }

            public SearchNode(
                DiscreteState discreteState,
                IState state,
                IAction? actionFromPreviousState,
                SearchNode? previousState,
                double costToCome,
                double estimatedTotalCost)
            {
                Key = discreteState;
                State = state;
                ActionFromPreviousState = actionFromPreviousState;
                PreviousNode = previousState;
                CostToCome = costToCome;
                EstimatedTotalCost = estimatedTotalCost;
            }

            public int CompareTo(SearchNode other)
            {
                var diff = EstimatedTotalCost - other.EstimatedTotalCost;
                return diff < 0 ? -1 : (diff == 0 ? 0 : 1);
            }
        }
    }
}