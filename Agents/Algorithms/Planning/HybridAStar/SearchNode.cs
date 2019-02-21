using Racing.Agents.Algorithms.Planning.HybridAStar.DataStructures;
using Racing.Model;
using System;
using System.Collections.Generic;

namespace Racing.Agents.Algorithms.Planning.HybridAStar
{
    internal sealed class SearchNode : ISearchNode<DiscreteState>, IComparable<SearchNode>
    {
        public DiscreteState Key { get; }
        public IState State { get; }
        public IAction? ActionFromPreviousState { get; }
        public SearchNode? PreviousNode { get; }
        public double CostToCome { get; }
        public double EstimatedTotalCost { get; }
        public IReadOnlyList<IGoal> RemainingWayPoints { get; }

        public SearchNode(
            DiscreteState discreteState,
            IState state,
            IAction? actionFromPreviousState,
            SearchNode? previousState,
            double costToCome,
            double estimatedTotalCost,
            IReadOnlyList<IGoal> remainingWayPoints)
        {
            Key = discreteState;
            State = state;
            ActionFromPreviousState = actionFromPreviousState;
            PreviousNode = previousState;
            CostToCome = costToCome;
            EstimatedTotalCost = estimatedTotalCost;
            RemainingWayPoints = remainingWayPoints;
        }

        public int CompareTo(SearchNode other)
        {
            var diff = EstimatedTotalCost - other.EstimatedTotalCost;
            return diff < 0 ? -1 : (diff == 0 ? 0 : 1);
        }

        public IPlan ReconstructPlan()
        {
            var trajectory = new List<IActionTrajectory>();
            SearchNode? node = this;

            trajectory.Add(new ActionTrajectory(TimeSpan.FromSeconds(node.CostToCome), State, null));

            while (node.PreviousNode != null)
            {
                trajectory.Add(
                    new ActionTrajectory(TimeSpan.FromSeconds(node.CostToCome), node.PreviousNode.State, node.ActionFromPreviousState));
                node = node.PreviousNode;
            }

            trajectory.Reverse();

            return new Plan(TimeSpan.FromSeconds(CostToCome), trajectory);
        }
    }
}