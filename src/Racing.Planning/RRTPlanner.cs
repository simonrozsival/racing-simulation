using Racing.Model;
using Racing.Model.CollisionDetection;
using Racing.Model.Vehicle;
using Racing.Planning.Algorithms.RRT;
using Racing.Planning.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

namespace Racing.Planning
{
    internal sealed class RRTPlanner : IPlanner
    {
        private readonly double goalBias;
        private readonly int maximumNumberOfIterations;
        private readonly IVehicleModel vehicleModel;
        private readonly IMotionModel motionModel;
        private readonly ITrack track;
        private readonly ICollisionDetector collisionDetector;
        private readonly Random random;
        private readonly TimeSpan timeStep;
        private readonly ISubject<IState> exploredStates = new Subject<IState>();
        private readonly IActionSet actions;
        private readonly IGoal goal;
        private readonly DistanceMeasurement distances;

        public IObservable<IState> ExploredStates { get; }

        public RRTPlanner(
            double goalBias,
            int maximumNumberOfIterations,
            IWorldDefinition world,
            Random random,
            TimeSpan timeStep,
            IGoal goal)
        {
            if (goalBias > 0.5)
            {
                throw new ArgumentOutOfRangeException($"Goal bias must be at most 0.5 (given {goalBias}).");
            }

            this.goalBias = goalBias;
            this.maximumNumberOfIterations = maximumNumberOfIterations;
            this.vehicleModel = world.VehicleModel;
            this.motionModel = world.MotionModel;
            this.track = world.Track;
            this.collisionDetector = world.CollisionDetector;
            this.random = random;
            this.timeStep = timeStep;
            this.goal = goal;
            this.actions = world.Actions;

            distances = new DistanceMeasurement(track.Width, track.Height);

            ExploredStates = exploredStates;
        }

        public IPlan? FindOptimalPlanFor(IState initialState)
        {
            var sampler = new Sampler(random, track, vehicleModel, goalBias);

            var nodes = new List<TreeNode>();
            nodes.Add(new TreeNode(initialState));

            for (int i = 0; i < maximumNumberOfIterations; i++)
            {
                var sampleState = sampler.RandomSampleOfFreeRegion(goal);
                var nearestNode = nearest(nodes, sampleState);

                if (nearestNode == null)
                {
                    // we tried every action of all the nodes which are in the tree and we can't produce
                    // any new states with the curreht timeStep
                    return null;
                }

                var (newState, selectedAction) = steer(nearestNode, sampleState, distances, out var reachedGoal);

                if (newState == null)
                {
                    continue;
                }

                var newNode = new TreeNode(nearestNode, newState, selectedAction, timeStep, 0);
                exploredStates.OnNext(newState);

                if (reachedGoal)
                {
                    return newNode.ReconstructPlanFromRoot();
                }

                nodes.Add(newNode);
            }

            return null;
        }

        private TreeNode? nearest(List<TreeNode> nodes, IState state)
        {
            // todo: use kd-tree

            TreeNode? best = null;
            var shortestDistance = double.MaxValue;
            var extendableNodes = nodes.Where(node => node.CanBeExpanded);

            foreach (var node in extendableNodes)
            {
                var currentNodeDistance = distances.DistanceBetween(node.State, state);
                if (currentNodeDistance < shortestDistance)
                {
                    best = node;
                    shortestDistance = currentNodeDistance;
                }
            }

            return best;
        }

        private (IState?, IAction?) steer(TreeNode from, IState to, DistanceMeasurement distances, out bool reachedGoal)
        {
            IState? state = null;
            IAction? bestAction = null;
            reachedGoal = false;
            var shortestDistance = double.MaxValue;

            // todo what if there are no available actions?
            var availableActions = from.SelectAvailableActionsFrom(actions.AllPossibleActions).ToArray();
            var remainingAvailableActions = availableActions.Length;
            foreach (var action in availableActions)
            {
                var predictedStates = motionModel.CalculateNextState(from.State, action, timeStep).ToList();
                var elapsedTime = predictedStates.Last().relativeTime;
                var resultState = predictedStates.Last().state;
                var passedGoal = false;
                var collided = false;

                foreach (var (simulationTime, intermediateState) in predictedStates)
                {
                    if (collisionDetector.IsCollision(intermediateState))
                    {
                        elapsedTime = simulationTime;
                        resultState = intermediateState;
                        passedGoal = false;
                        collided = true;
                        break;
                    }

                    if (goal.ReachedGoal(intermediateState.Position))
                    {
                        passedGoal = true;
                    }
                }

                if (collided)
                {
                    from.DisableAction(action);
                    remainingAvailableActions--;
                    continue;
                }

                var currentDistance = distances.DistanceBetween(to, resultState);
                if ((!reachedGoal || passedGoal) && currentDistance < shortestDistance)
                {
                    state = resultState;
                    bestAction = action;
                    reachedGoal = passedGoal;
                }
            }

            if (bestAction != null)
            {
                from.DisableAction(bestAction);
                remainingAvailableActions--;
            }

            if (bestAction == null || remainingAvailableActions == 0)
            {
                from.DisableFutureExpansions();
            }

            return (state, bestAction);
        }
    }
}
