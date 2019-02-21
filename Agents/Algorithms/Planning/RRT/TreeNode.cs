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

        public TreeNode(IState state)
        {
            Parent = null;
            State = state;
            ActionFromParent = null;
            CostToCome = TimeSpan.Zero;
            CanBeExpanded = true;
        }

        public TreeNode(TreeNode parent, IState state, IAction actionFromParent, TimeSpan costOfAction)
        {
            Parent = parent;
            State = state;
            ActionFromParent = actionFromParent;
            CostToCome = (Parent?.CostToCome ?? TimeSpan.Zero) + costOfAction;
            CanBeExpanded = true;
        }

        public IEnumerable<IAction> SelectAvailableActionsFrom(IEnumerable<IAction> allActions)
            => allActions.Where(action => !forbinnedActions.Contains(action));

        public IPlan ReconstructPlanFromRoot()
        {
            var actions = new List<IAction>();
            var states = new List<IState>();
            var timeToGoal = CostToCome;

            TreeNode? node = this;
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
