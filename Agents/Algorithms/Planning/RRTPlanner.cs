using Racing.Agents.Algorithms.Planning.RRT;
using Racing.Mathematics;
using Racing.Model;
using Racing.Model.CollisionDetection;
using Racing.Model.Vehicle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

namespace Racing.Agents.Algorithms.Planning
{
    internal sealed class RRTPlanner : IPlanner
    {
        private readonly double goalBias;
        private readonly int maximumNumberOfIterations;
        private readonly IVehicleModel vehicleModel;
        private readonly IMotionModel motionModel;
        private readonly ITrack track;
        private readonly Random random;
        private readonly TimeSpan timeStep;
        private readonly ISubject<IState> exploredStates = new Subject<IState>();
        private readonly ICollisionDetector collisionDetector;

        public IObservable<IState> ExploredStates { get; }

        public RRTPlanner(
            double goalBias,
            int maximumNumberOfIterations,
            IVehicleModel vehicleModel,
            IMotionModel motionModel,
            ITrack track,
            ICollisionDetector collisionDetector,
            Random random,
            TimeSpan timeStep)
        {
            if (goalBias > 0.5)
            {
                throw new ArgumentOutOfRangeException($"Goal bias must be at most 0.5 (given {goalBias}).");
            }

            this.goalBias = goalBias;
            this.maximumNumberOfIterations = maximumNumberOfIterations;
            this.vehicleModel = vehicleModel;
            this.motionModel = motionModel;
            this.track = track;
            this.collisionDetector = collisionDetector;
            this.random = random;
            this.timeStep = timeStep;

            ExploredStates = exploredStates;
        }

        public IPlan? FindOptimalPlanFor(PlanningProblem problem)
        {
            var sampler = new Sampler(random, track, vehicleModel, problem.Goal, goalBias);
            var distances = new DistanceMeasurement(problem.InitialState.Position, problem.Goal.Position);

            var nodes = new List<TreeNode>();
            nodes.Add(new TreeNode(problem.InitialState));

            for (int i = 0; i < maximumNumberOfIterations; i++)
            {
                Console.WriteLine($"{i} ({(double)i / maximumNumberOfIterations * 100}%)");
                var sampleState = sampler.RandomSampleOfFreeRegion();
                var nearestNode = nearest(nodes, sampleState, distances);

                if (nearestNode == null)
                {
                    // we tried every action of all the nodes which are in the tree and we can't produce
                    // any new states with the curreht timeStep
                    return null;
                }

                var (newState, selectedAction) = steer(nearestNode, sampleState, problem.Actions, distances);

                if (newState == null)
                {
                    continue;
                }

                var newNode = new TreeNode(nearestNode, newState, selectedAction, timeStep);

                exploredStates.OnNext(newState);

                if (problem.Goal.ReachedGoal(newState.Position))
                {
                    return newNode.ReconstructPlanFromRoot();
                }

                nodes.Add(newNode);
            }

            return null;
        }

        private TreeNode? nearest(List<TreeNode> nodes, IState state, DistanceMeasurement distances)
        {
            // todo: use kd-tree

            TreeNode? best = null;
            var shortestDistance = double.MaxValue;

            foreach (var node in nodes)
            {
                if (!node.CanBeExpanded)
                {
                    continue;
                }

                var currentNodeDistance = distances.DistanceBetween(node.State, state);
                if (currentNodeDistance < shortestDistance)
                {
                    best = node;
                    shortestDistance = currentNodeDistance;
                }
            }

            return best;
        }

        private (IState?, IAction?) steer(TreeNode from, IState to, IActionSet actions, DistanceMeasurement distances)
        {
            IState? state = null;
            IAction? bestAction = null;
            var shortestDistance = double.MaxValue;
            var distanceFromStartToGoal = (from.State.Position - to.Position).CalculateLength();

            // todo what if there are no available actions?
            var availableActions = from.SelectAvailableActionsFrom(actions.AllPossibleActions).ToArray();
            var remainingAvailableActions = availableActions.Length;
            foreach (var action in availableActions)
            {
                var resultState = motionModel.CalculateNextState(from.State, action, timeStep);
                if (collisionDetector.IsCollision(resultState))
                {
                    from.DisableAction(action);
                    remainingAvailableActions--;
                    continue;
                }

                var currentDistance = distances.DistanceBetween(to, resultState);
                if (currentDistance < shortestDistance)
                {
                    state = resultState;
                    bestAction = action;
                }
            }

            if (bestAction != null)
            {
                from.DisableAction(bestAction);
                remainingAvailableActions--;
            }

            if (remainingAvailableActions == 0)
            {
                from.DisableFutureExpansions();
            }

            return (state, bestAction);
        }
    }
}
