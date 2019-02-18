using System;
using System.Collections.Generic;

namespace Racing.Agents.Algorithms.Planning.AStar.DataStructures
{
    /// <summary>
    /// This data structure has O(n) dequeuing of the min element,
    /// but all the other operations are O(1).
    /// </summary>
    /// <typeparam name="T">Search node type</typeparam>
    internal sealed class HashTableOpenSet<T> : IOpenSet<T>
        where T : ISearchNode, IComparable<T>
    {
        private readonly Dictionary<long, T> hashTable = new Dictionary<long, T>();

        public void Add(T node)
        {
            hashTable.Add(node.Id, node);
        }

        public T DequeueMostPromissing()
        {
            long? bestKey = null;
            foreach (var pair in hashTable)
            {
                if (!bestKey.HasValue || pair.Value.CompareTo(hashTable[bestKey.Value]) == -1)
                {
                    bestKey = pair.Key;
                }
            }

            var best = hashTable[bestKey.Value]; // might throw, but I won't check for it myself
            hashTable.Remove(bestKey.Value);
            return best;
        }

        public bool IsEmpty => hashTable.Count == 0;

        public bool Contains(T node)
            => hashTable.ContainsKey(node.Id);

        public T NodeSimilarTo(T node)
            => hashTable[node.Id];

        public void Replace(T node)
            => hashTable[node.Id] = node;
    }
}
