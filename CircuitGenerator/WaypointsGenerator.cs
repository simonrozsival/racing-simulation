using Racing.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using static Racing.Mathematics.CustomMath;

namespace Racing.CircuitGenerator
{
    internal sealed class WaypointsGenerator
    {
        private readonly int min;
        private readonly int max;
        private readonly Random random;
        private readonly double minimumDistance;

        public WaypointsGenerator(int min, int max, Random random, double minimumDistance)
        {
            this.min = min;
            this.max = max;
            this.random = random;
            this.minimumDistance = minimumDistance;
        }

        public IEnumerable<Vector> GenerateSimpleCircuit(double width, double height)
        {
            yield return new Vector(0.5 * width, minimumDistance);
            yield return new Vector(width - minimumDistance, minimumDistance);
            yield return new Vector(width - minimumDistance, 0.5 * height);
            yield return new Vector(width - minimumDistance, height - minimumDistance);
            yield return new Vector(0.5 * width, height - minimumDistance);
            yield return new Vector(minimumDistance, height - minimumDistance);
            yield return new Vector(minimumDistance, 0.5 * height);
            yield return new Vector(minimumDistance, minimumDistance);
        }

        public IEnumerable<Vector> Generate(double width, double height)
        {
            var numberOfPoints = random.Next(min, max);

            var randomPoints = generateRandomPoints(numberOfPoints, width, height);

            var dataSet = computeConvexHull(randomPoints);

            pushApart(dataSet, n: 5);
            dataSet = insertRandomMidpoints(dataSet);
            pushApart(dataSet, n: 5);
            trySelectPointOnTheLongestStraightAsStart(dataSet);

            return dataSet;
        }

        private List<Vector> generateRandomPoints(int numberOfPoints, double width, double height)
            => Enumerable.Range(0, numberOfPoints)
                .Select(_ => new Vector(
                    x: 2 * minimumDistance + random.NextDouble() * (width - 4 * minimumDistance),
                    y: 2 * minimumDistance + random.NextDouble() * (height - 4 * minimumDistance)))
                .ToList();

        private static List<Vector> computeConvexHull(List<Vector> points)
        {
            points.Sort(
                (a, b) => a.X == b.X
                    ? a.Y.CompareTo(b.Y)
                    : -a.X.CompareTo(b.X));

            var hull = new List<Vector>();
            int L = 0, U = 0;

            for (int i = points.Count - 1; i >= 0; i--)
            {
                Vector p = points[i], p1;

                while (L >= 2 && ((p1 = hull.Last()) - hull[hull.Count - 2]).Cross(p - p1) >= 0)
                {
                    hull.RemoveAt(hull.Count - 1);
                    L--;
                }

                hull.Add(p);
                L++;

                while (U >= 2 && ((p1 = hull.First()) - hull[1]).Cross(p - p1) <= 0)
                {
                    hull.RemoveAt(0);
                    U--;
                }

                if (U != 0)
                {
                    hull.Insert(0, p);
                }

                U++;
            }

            hull.RemoveAt(hull.Count - 1);
            hull.Reverse();

            return hull.ToList();
        }

        private void pushApart(IList<Vector> points, int n)
        {
            for (var i = 0; i < n; i++)
            {
                pushApart(points);
            }
        }

        private void pushApart(IList<Vector> points)
        {
            var dst2 = minimumDistance * minimumDistance;
            for (int i = 0; i < points.Count; ++i)
            {
                for (int j = i + 1; j < points.Count; ++j)
                {
                    if (points[i].DistanceSq(points[j]) < dst2)
                    {
                        var direction = points[j] - points[i];
                        var distance = direction.CalculateLength();
                        var shiftDistance = minimumDistance - distance;
                        var displacement = shiftDistance * direction.Normalize();

                        points[j] = points[j] + displacement;
                        points[i] = points[i] - displacement;
                    }
                }
            }
        }

        private List<Vector> insertRandomMidpoints(IList<Vector> dataSet)
        {
            var extendedDataSet = new List<Vector>(dataSet.Count * 2);
            var difficulty = random.NextDouble();
            var maxDisp = minimumDistance / 2;
            for (int i = 0; i < dataSet.Count; ++i)
            {
                var displacementLength = (float)Math.Pow(random.NextDouble(), difficulty) * maxDisp;

                var displacement = new Vector(0, 1).Rotate(random.NextDouble() * 2 * Math.PI);

                var midpoint = 0.5 * (dataSet[i] + dataSet[(i + 1) % dataSet.Count]);
                extendedDataSet.Add(dataSet[i]);
                extendedDataSet.Add(midpoint + displacementLength * displacement);
            }

            return extendedDataSet;
        }

        private static void trySelectPointOnTheLongestStraightAsStart(List<Vector> dataSet)
        {
            var maxLength = Length.FromMeters(-1.0);
            var maxIndex = -1;

            for (int i = 0; i < dataSet.Count; i++)
            {
                var prev = dataSet[(i - 1 + dataSet.Count) % dataSet.Count];
                var next = dataSet[(i + 1) % dataSet.Count];
                var curr = dataSet[i];

                var a = curr - prev;
                var b = next - curr;

                var lengthA = a.CalculateLength();
                var lengthB = b.CalculateLength();
                var angle = Acos(a.Dot(b) / (lengthA * lengthB));

                if (angle < Math.PI / 6 && maxLength < lengthA + lengthB)
                {
                    maxLength = lengthA + lengthB;
                    maxIndex = i;
                }
            }

            if (maxIndex > 0)
            {
                for (int i = 0; i < maxIndex; i++)
                {
                    var usedToBeFirst = dataSet[0];
                    dataSet.RemoveAt(0);
                    dataSet.Add(usedToBeFirst);
                }
            }
        }
    }
}
