using System;

namespace Racing.Agents.Algorithms.Planning.HybridAStar.DataStructures
{
    internal interface ISearchNode<TKey>
        where TKey: IEquatable<TKey>
    {
        TKey Key { get; }
        double EstimatedTotalCost { get; }
    }
}
