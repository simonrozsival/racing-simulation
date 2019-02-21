using Racing.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Racing.Agents.Algorithms.Planning.RRT
{
    internal sealed class TreeNode
    {
        private readonly HashSet<IAction> forbinnedActions = new HashSet<IAction>();

        public TreeNode? Parent { get; }
        public IState State { get; }
        public IAction? ActionFromParent { get; }
        public TimeSpan CostToCome { get; }
        public bool CanBeExpanded { get; private set; }
        public int WayPointsReached { get; }

        public TreeNode(IState state)
        {
            Parent = null;
            State = state;
            ActionFromParent = null;
            CostToCome = TimeSpan.Zero;
            CanBeExpanded = true;
            WayPointsReached = 0;
        }

        public TreeNode(
            TreeNode parent,
            IState state,
            IAction actionFromParent,
            TimeSpan costOfAction,
            int wayPointsReached)
        {
            Parent = parent;
            State = state;
            ActionFromParent = actionFromParent;
            CostToCome = (Parent?.CostToCome ?? TimeSpan.Zero) + costOfAction;
            CanBeExpanded = true;
            WayPointsReached = wayPointsReached;
        }

        public IEnumerable<IAction> SelectAvailableActionsFrom(IEnumerable<IAction> allActions)
            => allActions.Where(action => !forbinnedActions.Contains(action));

        public IPlan ReconstructPlanFromRoot()
        {
            var trajectory = new List<IActionTrajectory>();
            TreeNode? node = this;

            trajectory.Add(new ActionTrajectory(node.CostToCome, State, null));

            while (node.Parent != null)
            {
                trajectory.Add(
                    new ActionTrajectory(node.CostToCome, node.Parent.State, node.ActionFromParent));
                node = node.Parent;
            }

            trajectory.Reverse();

            return new Plan(CostToCome, trajectory);
        }

        public void DisableAction(IAction action)
        {
            forbinnedActions.Add(action);
        }

        public void DisableFutureExpansions()
        {
            CanBeExpanded = false;
        }
    }
}
