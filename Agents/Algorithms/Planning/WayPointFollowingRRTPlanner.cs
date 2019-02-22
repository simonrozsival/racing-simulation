using Racing.Agents.Algorithms.Planning.RRT;
using Racing.Model;
using Racing.Model.CollisionDetection;
using Racing.Model.Vehicle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

namespace Racing.Agents.Algorithms.Planning
{
    internal sealed class WayPointsFollowingRRTPlanner : IPlanner
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
        private readonly IReadOnlyList<IGoal> wayPoints;
        private readonly DistanceMeasurement distances;

        private int wayPointsReached;

        public IObservable<IState> ExploredStates { get; }

        public WayPointsFollowingRRTPlanner(
            double goalBias,
            int maximumNumberOfIterations,
            IVehicleModel vehicleModel,
            IMotionModel motionModel,
            ITrack track,
            ICollisionDetector collisionDetector,
            Random random,
            TimeSpan timeStep,
            IActionSet actions,
            IReadOnlyList<IGoal> wayPoints)
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
            this.wayPoints = wayPoints;
            this.actions = actions;

            distances = new DistanceMeasurement(track.Width, track.Height);
            wayPointsReached = 0;

            ExploredStates = exploredStates;
        }

        public IPlan? FindOptimalPlanFor(IState initialState)
        {
            var sampler = new Sampler(random, track, vehicleModel, goalBias);

            var nodes = new List<TreeNode>();
            nodes.Add(new TreeNode(initialState));

            for (int i = 0; i < maximumNumberOfIterations; i++)
            {
                var sampleState = sampler.RandomSampleOfFreeRegion(wayPoints[wayPointsReached]);
                var nearestNode = nearest(nodes, sampleState);

                if (nearestNode == null)
                {
                    // we tried every action of all the nodes which are in the tree and we can't produce
                    // any new states with the curreht timeStep
                    return null;
                }

                var (newState, selectedAction, reachedBreakPoint) = steer(nearestNode, sampleState, distances);

                if (newState == null)
                {
                    continue;
                }

                var reachedWayPoints = reachedBreakPoint
                    ? nearestNode.WayPointsReached + 1
                    : nearestNode.WayPointsReached;

                if (reachedWayPoints > wayPointsReached)
                {
                    wayPointsReached = reachedWayPoints;
                }

                var newNode = new TreeNode(nearestNode, newState, selectedAction, timeStep, reachedWayPoints);
                exploredStates.OnNext(newState);

                if (reachedWayPoints == wayPoints.Count)
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

        private (IState?, IAction?, bool) steer(TreeNode from, IState to, DistanceMeasurement distances)
        {
            IState? state = null;
            IAction? bestAction = null;
            bool reachedWayPoint = false;
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

                    if (wayPoints[wayPointsReached].ReachedGoal(intermediateState.Position))
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
                if ((!reachedWayPoint || passedGoal) && currentDistance < shortestDistance)
                {
                    state = resultState;
                    bestAction = action;
                    reachedWayPoint = passedGoal;
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

            return (state, bestAction, reachedWayPoint);
        }
    }
}
