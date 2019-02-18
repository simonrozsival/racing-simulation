using System;

namespace Racing.Agents.Algorithms.Planning.AStar.DataStructures
{
    internal interface IOpenSet<T>
        where T : ISearchNode, IComparable<T>
    {
        bool IsEmpty { get; }
        bool Contains(T node);
        T NodeSimilarTo(T node);
        void Add(T node);
        void Replace(T node);
        T DequeueMostPromissing();
    }
}
