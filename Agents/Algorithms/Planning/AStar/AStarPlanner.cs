using Racing.Agents.Algorithms.Planning.AStar.DataStructures;
using Racing.Agents.Algorithms.Planning.AStar.Heuristics;
using Racing.Model;
using Racing.Model.CollisionDetection;
using System;
using System.Collections.Generic;

namespace Racing.Agents.Algorithms.Planning
{
    internal class AStarPlanner : IPlanner
    {
        private readonly TimeSpan timeStep;
        private readonly TimeSpan simulationStep;
        private readonly int simulationsPerStep;
        private readonly BoundingSphereCollisionDetector collisionDetector;

        public AStarPlanner(
            BoundingSphereCollisionDetector collisionDetector,
            TimeSpan timeStep,
            TimeSpan simulationStep)
        {
            if (Math.Floor(timeStep / simulationStep) != (timeStep / simulationStep))
            {
                throw new ArgumentException($"Time step must be divisible by simulation step.");
            }

            simulationsPerStep = (int)(timeStep / simulationStep);

            if (simulationsPerStep < 2)
            {
                throw new ArgumentException($"There must be at least 2 simulation steps per one time step (given {simulationsPerStep}).");
            }

            this.timeStep = timeStep;
            this.simulationStep = simulationStep;
            this.collisionDetector = collisionDetector;
        }

        public IEnumerable<IAction> FindOptimalPlanFor(PlanningProblem problem)
        {
            // var heuristic = createShortestPathHeuristic(problem);
            var heuristic = new EuclideanDistanceHeuristic();
            // var heuristic = new DijkstraAkaNoHeuristic();

            var open = new BinaryHeapOpenSet<SearchNode>();
            //var open = new HashTableOpenSet<SearchNode>();
            var closed = new ClosedSet<long>();

            void exporeLater(SearchNode node)
            {
                if (!open.Contains(node))
                {
                    open.Add(node);
                }
                else if (node.TimeNeededFromStartToThisState < open.NodeSimilarTo(node).TimeNeededFromStartToThisState)
                {
                    open.Replace(node);
                }
            }

            var exploreNext =
                new SearchNode(
                    state: problem.InitialState,
                    actionFromPreviousState: null,
                    previousState: null,
                    timeNeededFromStartToThisState: 0,
                    estimatedCost: heuristic.EstimateTimeToGoal(problem.InitialState, problem).TotalSeconds);

            while (exploreNext != null || !open.IsEmpty)
            {
                var expandedNode = exploreNext;
                if (expandedNode == null)
                {
                    expandedNode = open.DequeueMostPromissing();
                }
                else
                {
                    exploreNext = null;
                }

                if (problem.Goal.ReachedGoal(expandedNode.State.Position))
                {
                    return reconstructActions(expandedNode);
                }

                closed.Add(expandedNode.Id);

                foreach (var action in problem.PossibleActions)
                {
                    var timeSpentOnManeuver = TimeSpan.Zero;
                    var nextVehicleState = expandedNode.State;
                    for (int i = 0; i < simulationsPerStep; i++)
                    {
                        nextVehicleState = problem.MotionModel.CalculateNextState(nextVehicleState, action, simulationStep);
                        timeSpentOnManeuver += simulationStep;
                        if (collisionDetector.IsCollision(nextVehicleState.Position))
                        {
                            closed.Add(hash(nextVehicleState));
                            break;
                        }

                        if (problem.Goal.ReachedGoal(nextVehicleState.Position))
                        {
                            break;
                        }
                    }

                    if (closed.Contains(hash(nextVehicleState)))
                    {
                        continue;
                    }

                    if (collisionDetector.IsCollision(nextVehicleState.Position))
                    {
                        closed.Add(hash(nextVehicleState));
                        continue;
                    }

                    // time can be different from one whole "step" because the simulation could have broken at some sub-step
                    // if it reached a goal somewhere in the middle of the time step.
                    var timeFromStart = expandedNode.TimeNeededFromStartToThisState + timeSpentOnManeuver.TotalSeconds;
                    var discoveredNode = new SearchNode(
                        state: nextVehicleState,
                        actionFromPreviousState: action,
                        previousState: expandedNode,
                        timeNeededFromStartToThisState: timeFromStart,
                        estimatedCost: timeFromStart + heuristic.EstimateTimeToGoal(nextVehicleState, problem).TotalSeconds);

                    if (!open.Contains(discoveredNode))
                    {
                        if (discoveredNode.EstimatedCost < expandedNode.EstimatedCost)
                        {
                            if (exploreNext == null)
                            {
                                exploreNext = discoveredNode;
                            }
                            else if (discoveredNode.EstimatedCost < exploreNext.EstimatedCost)
                            {
                                exporeLater(exploreNext);
                                exploreNext = discoveredNode;
                            }
                        }
                        else
                        {
                            exporeLater(discoveredNode);
                        }
                    }
                    else
                    {
                        exporeLater(discoveredNode);
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
                problem.Environment,
                collisionDetector,
                stepSize: problem.VehicleModel.Length,
                maxSpeed: problem.VehicleModel.MaxVelocity);

            return heuristic;
        }

        private IEnumerable<IAction> reconstructActions(SearchNode state)
        {
            var actions = new List<IAction>();
            while (state != null && state.ActionFromPreviousState != null)
            {
                actions.Insert(0, state.ActionFromPreviousState);
                state = state.PreviousState;
            }

            return actions;
        }

        private sealed class SearchNode : ISearchNode, IComparable<SearchNode>
        {
            public long Id { get; }
            public IState State { get; }
            public IAction ActionFromPreviousState { get; }
            public SearchNode PreviousState { get; }
            public double TimeNeededFromStartToThisState { get; }
            public double EstimatedCost { get; }

            public SearchNode(
                IState state,
                IAction actionFromPreviousState,
                SearchNode previousState,
                double timeNeededFromStartToThisState,
                double estimatedCost)
            {
                Id = hash(state);
                State = state;
                ActionFromPreviousState = actionFromPreviousState;
                PreviousState = previousState;
                TimeNeededFromStartToThisState = timeNeededFromStartToThisState;
                EstimatedCost = estimatedCost;
            }

            public int CompareTo(SearchNode other)
            {
                var diff = EstimatedCost - other.EstimatedCost;
                return diff < 0 ? -1 : (diff == 0 ? 0 : 1);
            }
        }

        private static long hash(IState state)
        {
            const double resolution = 5;
            const double angularResolution = (2 * Math.PI) / 12;

            var x = (long)(state.Position.X / resolution);
            var y = (long)(state.Position.Y / resolution) * 10000;
            var a = (long)(state.HeadingAngle.Radians / angularResolution) * 10000 * 10000;
            return x + y + a;
        }
    }
}