using System.Collections.Generic;

namespace Racing.Agents.Algorithms.Planning.AStar.DataStructures
{
    internal sealed class ClosedSet<T>
    {
        private readonly HashSet<T> closed = new HashSet<T>();

        public bool Contains(T item)
            => closed.Contains(item);

        public void Add(T item)
        {
            closed.Add(item);
        }
    }
}
