using System;

namespace Racing.Planning.Algorithms.HybridAStar.DataStructures
{
    internal interface ISearchNode<TKey>
        where TKey: IEquatable<TKey>
    {
        TKey Key { get; }
        double EstimatedTotalCost { get; }
    }
}
