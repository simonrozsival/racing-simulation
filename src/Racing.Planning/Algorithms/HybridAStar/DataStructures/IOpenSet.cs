using System;

namespace Racing.Planning.Algorithms.HybridAStar.DataStructures
{
    internal interface IOpenSet<TKey, TValue>
        where TValue : ISearchNode<TKey>, IComparable<TValue>
        where TKey : IEquatable<TKey>
    {
        bool IsEmpty { get; }
        bool Contains(TKey key);
        TValue Get(TKey key);
        void Add(TValue node);
        void ReplaceExistingWithTheSameKey(TValue node);
        TValue DequeueMostPromissing();
    }
}
