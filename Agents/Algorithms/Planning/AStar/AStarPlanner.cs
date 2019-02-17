using Racing.Agents.Algorithms.Planning.AStar.DataStructures;
using Racing.Agents.Algorithms.Planning.AStar.Heuristics;
using Racing.Model;
using Racing.Model.CollisionDetection;
using Racing.Model.Vehicle;
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
            //var heuristic = createShortestPathHeuristic(problem);
            var heuristic = new EuclideanDistanceHeuristic();

            var open = new OpenSet<SearchNode>();
            var closed = new ClosedSet<long>();

            open.Add(
                new SearchNode(
                    state: problem.InitialState,
                    actionFromPreviousState: null,
                    previousState: null,
                    timeNeededFromStartToThisState: 0,
                    estimatedCost: heuristic.EstimateTimeToGoal(problem.InitialState, problem).TotalSeconds));

            while (!open.IsEmpty)
            {
                var expandedNode = open.DequeueMostPromissingState();

                if (problem.Goal.ReachedGoal(expandedNode.State.Position))
                {
                    return reconstructActions(expandedNode);
                }

                closed.Add(expandedNode.Id);

                foreach (var action in problem.PossibleActions)
                {
                    var nextVehicleState = expandedNode.State;
                    for (int i = 0; i < simulationsPerStep; i++)
                    {
                        nextVehicleState = problem.MotionModel.CalculateNextState(nextVehicleState, action, simulationStep);
                        if (collisionDetector.IsCollision(nextVehicleState.Position))
                        {
                            closed.Add(hash(nextVehicleState));
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

                    var timeFromStart = expandedNode.TimeNeededFromStartToThisState + timeStep.TotalSeconds;
                    var next = new SearchNode(
                        state: nextVehicleState,
                        actionFromPreviousState: action,
                        previousState: expandedNode,
                        timeNeededFromStartToThisState: timeFromStart,
                        estimatedCost: timeFromStart + heuristic.EstimateTimeToGoal(nextVehicleState, problem).TotalSeconds);

                    if (!open.Contains(next))
                    {
                        open.Add(next);
                    }
                    else
                    {
                        if (next.TimeNeededFromStartToThisState < open.AlreadyStoredStateCloseTo(next).TimeNeededFromStartToThisState)
                        {
                            open.Replace(next);
                        }
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
            const double angularResolution = (2 * Math.PI) / 6;

            var x = (long)(state.Position.X / resolution);
            var y = (long)(state.Position.Y / resolution) * 10000;
            var a = (long)(state.HeadingAngle.Radians / angularResolution) * 10000 * 10000;
            return x + y + a;
        }
    }
}