using System;
using System.Collections.Generic;

namespace Racing.Agents.Algorithms.Planning.AStar.DataStructures
{
    internal sealed class OpenSet<T>
        where T : ISearchNode, IComparable<T>
    {
        private readonly Dictionary<long, T> allStates = new Dictionary<long, T>();
        private readonly BinaryHeap<T> costs = new BinaryHeap<T>();

        public bool IsEmpty => allStates.Count == 0;

        public bool Contains(T state)
            => allStates.ContainsKey(state.Id);

        public T AlreadyStoredStateCloseTo(T state)
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

        public T DequeueMostPromissingState()
        {
            var next = costs.DequeueMin();
            allStates.Remove(next.Id);
            return next;
        }
    }
}
