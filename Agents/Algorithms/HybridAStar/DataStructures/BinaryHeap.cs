using System;
using System.Collections.Generic;

namespace Racing.Planning.Algorithms.HybridAStar.DataStructures
{
    internal sealed class BinaryHeap<T>
        where T : IComparable<T>
    {
        private readonly List<T> data = new List<T>();

        public bool IsEmpty => data.Count == 0;

        public T DequeueMin()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("The binary heap is empty.");
            }

            var minimum = data[0];
            data[0] = data[data.Count - 1];
            data.RemoveAt(data.Count - 1);
            bubbleDown(0);
            return minimum;
        }

        public void Add(T item)
        {
            data.Add(item);
            bubbleUp(data.Count - 1);
        }

        public void Replace(T old, T next)
        {
            var index = data.IndexOf(old);
            data[index] = next;
            index = bubbleUp(index);
            bubbleDown(index);
        }

        private int bubbleUp(int i)
        {
            bool swapped;
            do
            {
                swapped = false;
                var p = parent(i);
                if (data[i].CompareTo(data[p]) == -1)
                {
                    swap(i, p);
                    i = p;
                    swapped = true;
                }
            } while (swapped);

            return i;
        }

        private int bubbleDown(int i)
        {
            bool swapped;
            do
            {
                swapped = false;
                var l = left(i);
                var r = right(i);

                if (l < data.Count)
                {
                    if (r < data.Count)
                    {
                        if (data[i].CompareTo(data[l]) == 1 || data[i].CompareTo(data[r]) == 1)
                        {
                            if (data[l].CompareTo(data[r]) == -1)
                            {
                                swap(i, l);
                                i = l;
                            }
                            else
                            {
                                swap(i, r);
                                i = r;
                            }
                            swapped = true;
                        }
                    }
                    else if (data[i].CompareTo(data[l]) == 1)
                    {
                        swap(i, l);
                        i = l;
                        swapped = true;
                    }
                }
                else if (r < data.Count)
                {
                    if (data[i].CompareTo(data[r]) == 1)
                    {
                        swap(i, r);
                        i = r;
                        swapped = true;
                    }
                }
            } while (swapped);

            return i;
        }

        private void swap(int i, int j)
        {
            var tmp = data[i];
            data[i] = data[j];
            data[j] = tmp;
        }

        private int parent(int i) => i / 2;
        private int left(int i) => 2 * i;
        private int right(int i) => 2 * i + 1;
    }
}
