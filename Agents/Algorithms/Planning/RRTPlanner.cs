using Racing.Mathematics;
using Racing.Model;
using Racing.Model.CollisionDetection;
using Racing.Model.Vehicle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using static System.Math;

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
            var nodes = new List<TreeNode>();
            nodes.Add(new TreeNode(problem.InitialState));

            var distanceFromStartToToGoal = (problem.InitialState.Position - problem.Goal.Position).CalculateLength();

            for (int i = 0; i < maximumNumberOfIterations; i++)
            {
                Console.WriteLine($"{i} ({(double)i / maximumNumberOfIterations * 100}%)");
                var sampleState = selectRandomSample(random.NextDouble() > goalBias ? (Point?)null : problem.Goal.Position);
                var nearestNode = nearest(nodes, sampleState, distanceFromStartToToGoal);
                var (newState, selectedAction) = steer(nearestNode, sampleState, problem.Actions);

                if (newState == null)
                {
                    continue;
                }

                var newNode = new TreeNode(nearestNode, newState, selectedAction, timeStep);

                exploredStates.OnNext(newState);

                if (problem.Goal.ReachedGoal(newState.Position))
                {
                    return reconstructPlan(newNode);
                }

                nodes.Add(newNode);
            }

            return null;
        }

        private IState selectRandomSample(Point? position)
        {
            var freePosition = position ?? randomFrePosition();
            return new RandomState(
                position: freePosition,
                headingAngle: random.NextDoubleBetween(0, 2 * PI),
                steeringAngle: random.NextDoubleBetween(vehicleModel.MinSteeringAngle.Radians, vehicleModel.MaxSteeringAngle.Radians),
                speed: random.NextDoubleBetween(vehicleModel.MinSpeed, vehicleModel.MaxSpeed));
        }

        private Point randomFrePosition()
        {
            Point position;
            do
            {
                position = new Point(random.NextDoubleBetween(0, track.Width), random.NextDoubleBetween(0, track.Height));
            } while (track.IsOccupied(position.X, position.Y));

            return position;
        }

        private TreeNode nearest(List<TreeNode> nodes, IState state, double distanceFromStartToGoal)
        {
            // todo: use kd-tree

            TreeNode? best = null;
            var shortestDistance = double.MaxValue;

            foreach (var node in nodes)
            {
                var currentNodeDistance = distance(node.State, state, distanceFromStartToGoal);
                if (currentNodeDistance < shortestDistance)
                {
                    best = node;
                    shortestDistance = currentNodeDistance;
                }
            }

            if (best == null)
            {
                throw new InvalidOperationException($"Something is wrong. The list of nodes cannot be empty.");
            }

            return best;
        }

        private (IState?, IAction?) steer(TreeNode from, IState to, IActionSet actions)
        {
            IState? state = null;
            IAction? bestAction = null;
            var shortestDistance = double.MaxValue;
            var distanceFromStartToGoal = (from.State.Position - to.Position).CalculateLength();

            // todo what if there are no available actions?

            foreach (var action in from.SelectAvailableActionsFrom(actions.AllPossibleActions))
            {
                var resultState = motionModel.CalculateNextState(from.State, action, timeStep);
                if (collisionDetector.IsCollision(resultState))
                {
                    continue;
                }

                var currentDistance = distance(to, resultState, distanceFromStartToGoal);
                if (currentDistance < shortestDistance)
                {
                    state = resultState;
                    bestAction = action;
                }
            }

            return (state, bestAction);
        }

        private IPlan reconstructPlan(TreeNode? node)
        {
            var actions = new List<IAction>();
            var states = new List<IState>();
            var timeToGoal = node.CostToCome;

            while (node != null)
            {
                states.Insert(0, node.State);
                if (node.ActionFromParent != null)
                {
                    actions.Insert(0, node.ActionFromParent);
                }

                node = node.Parent;
            }

            return new Plan(timeToGoal, states, actions);
        }

        private static double distance(IState a, IState b, double distanceFromStartToGoal)
        {
            var euklideanDistance = distance(a.Position, b.Position, distanceFromStartToGoal);
            var angleDifference = distance(a.HeadingAngle, b.HeadingAngle);
            return Sqrt(euklideanDistance * euklideanDistance + angleDifference * angleDifference);
        }

        private static double distance(Point a, Point b, double distanceFromStartToGoal)
            => (a - b).CalculateLength() / distanceFromStartToGoal;

        private static double distance(Angle a, Angle b)
        {
            (a, b) = a.Radians < b.Radians ? (a, b) : (b, a);
            return Min(b.Radians - a.Radians, a.Radians + (2 * PI) - b.Radians) / (2 * PI);
        }

        private sealed class TreeNode
        {
            private readonly HashSet<IAction> usedActions = new HashSet<IAction>();

            public TreeNode? Parent { get; }
            public IState State { get; }
            public IAction? ActionFromParent { get; }
            public TimeSpan CostToCome { get; }
            public bool CanBeExpanded { get; }

            public TreeNode(IState state)
            {
                Parent = null;
                State = state;
                ActionFromParent = null;
                CostToCome = TimeSpan.Zero;
            }

            public TreeNode(TreeNode parent, IState state, IAction actionFromParent, TimeSpan costOfAction)
            {
                Parent = parent;
                State = state;
                ActionFromParent = actionFromParent;
                CostToCome = (Parent?.CostToCome ?? TimeSpan.Zero) + costOfAction;

                // update internal state of the parent
                Parent.usedActions.Add(actionFromParent);
            }

            public IEnumerable<IAction> SelectAvailableActionsFrom(IEnumerable<IAction> allActions)
                => allActions.Where(action => !usedActions.Contains(action));
        }

        private sealed class RandomState : IState
        {
            public RandomState(Point position, Angle headingAngle, Angle steeringAngle, double speed)
            {
                Position = position;
                HeadingAngle = headingAngle;
                SteeringAngle = steeringAngle;
                Speed = speed;
            }

            public Point Position { get; }
            public Angle HeadingAngle { get; }
            public Angle SteeringAngle { get; }
            public double Speed { get; }
        }
    }
}
