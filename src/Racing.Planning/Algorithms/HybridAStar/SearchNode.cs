using Racing.Model;
using Racing.Planning.Algorithms.HybridAStar.DataStructures;
using Racing.Planning.Domain;
using System;
using System.Collections.Generic;

namespace Racing.Planning.Algorithms.HybridAStar
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
            IAction? action = ActionFromPreviousState;

            while (node != null)
            {
                trajectory.Add(
                    new ActionTrajectory(TimeSpan.FromSeconds(node.CostToCome), node.State, action));
                action = node.ActionFromPreviousState;
                node = node.PreviousNode;
            }

            trajectory.Reverse();

            return new Plan(TimeSpan.FromSeconds(CostToCome), trajectory);
        }
    }
}