using System;

namespace Racing.Agents.Algorithms.Planning.AStar.DataStructures
{
    internal interface ISearchNode
    {
        long Id { get; }
        double EstimatedCost { get; }
    }
}
