using System;
using System.Collections.Generic;

namespace Racing.Agents.Algorithms.Planning.AStar.DataStructures
{
    /// <summary>
    /// This data structure has all operations at most O(log n).
    /// </summary>
    /// <typeparam name="T">Search node type</typeparam>
    internal sealed class BinaryHeapOpenSet<T> : IOpenSet<T>
        where T : ISearchNode, IComparable<T>
    {
        private readonly Dictionary<long, T> allStates = new Dictionary<long, T>();
        private readonly BinaryHeap<T> costs = new BinaryHeap<T>();

        public bool IsEmpty => allStates.Count == 0;

        public bool Contains(T state)
            => allStates.ContainsKey(state.Id);

        public T NodeSimilarTo(T state)
            => allStates[state.Id];

        public void Add(T state)
        {
            costs.Add(state);
            allStates.Add(state.Id, state);
        }

        public void Replace(T state)
        {
            var removedState = allStates[state.Id];
            allStates[state.Id] = state;
            costs.Replace(removedState, state);
        }

        public T DequeueMostPromissing()
        {
            var next = costs.DequeueMin();
            allStates.Remove(next.Id);
            return next;
        }
    }
}
