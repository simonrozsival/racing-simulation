using Racing.Model;
using Racing.Model.Planning;
using Racing.Model.Vehicle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Racing.Planning.Algorithms.RRT
{
    internal sealed class TreeNode
    {
        private readonly HashSet<IAction> forbinnedActions = new HashSet<IAction>();

        public TreeNode? Parent { get; }
        public VehicleState State { get; }
        public IAction? ActionFromParent { get; }
        public TimeSpan CostToCome { get; }
        public bool CanBeExpanded { get; private set; }
        public int TargetWayPoint { get; }

        public TreeNode(VehicleState state)
        {
            Parent = null;
            State = state;
            ActionFromParent = null;
            CostToCome = TimeSpan.Zero;
            CanBeExpanded = true;
            TargetWayPoint = 0;
        }

        public TreeNode(
            TreeNode parent,
            VehicleState state,
            IAction actionFromParent,
            TimeSpan costOfAction,
            int targetWayPoint)
        {
            Parent = parent;
            State = state;
            ActionFromParent = actionFromParent;
            CostToCome = (Parent?.CostToCome ?? TimeSpan.Zero) + costOfAction;
            CanBeExpanded = true;
            TargetWayPoint = targetWayPoint;
        }

        public IEnumerable<IAction> SelectAvailableActionsFrom(IEnumerable<IAction> allActions)
            => allActions.Where(action => !forbinnedActions.Contains(action));

        public IPlan ReconstructPlanFromRoot()
        {
            var trajectory = new List<IActionTrajectory>();
            TreeNode? node = this;

            trajectory.Add(new ActionTrajectory(node.CostToCome, State, null, node.TargetWayPoint));

            while (node.Parent != null)
            {
                trajectory.Add(
                    new ActionTrajectory(node.CostToCome, node.Parent.State, node.ActionFromParent, node.TargetWayPoint));
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
