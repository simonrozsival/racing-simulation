using System;
using System.Collections.Generic;

namespace Racing.Agents.Algorithms.Planning.HybridAStar.DataStructures
{
    /// <summary>
    /// This data structure has all operations at most O(log n).
    /// </summary>
    /// <typeparam name="TValue">Search node type</typeparam>
    internal sealed class BinaryHeapOpenSet<TKey, TValue> : IOpenSet<TKey, TValue>
        where TValue : ISearchNode<TKey>, IComparable<TValue>
        where TKey : IEquatable<TKey>
    {
        private readonly Dictionary<TKey, TValue> allStates = new Dictionary<TKey, TValue>();
        private readonly BinaryHeap<TValue> costs = new BinaryHeap<TValue>();

        public bool IsEmpty => allStates.Count == 0;

        public bool Contains(TKey key)
            => allStates.ContainsKey(key);

        public TValue Get(TKey key)
            => allStates[key];

        public void ReplaceExistingWithTheSameKey(TValue node)
        {
            var removedState = allStates[node.Key];
            allStates[node.Key] = node;
            costs.Replace(removedState, node);
        }

        public void Add(TValue state)
        {
            costs.Add(state);
            allStates.Add(state.Key, state);
        }

        public TValue DequeueMostPromissing()
        {
            var next = costs.DequeueMin();
            allStates.Remove(next.Key);
            return next;
        }
    }
}
