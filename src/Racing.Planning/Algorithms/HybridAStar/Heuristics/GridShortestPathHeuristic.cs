using Racing.Model;
using Racing.Mathematics;
using System;
using Racing.Model.CollisionDetection;
using System.Collections.Generic;
using System.Linq;
using Racing.Planning.Algorithms.HybridAStar.DataStructures;
using Racing.Model.Vehicle;

namespace Racing.Planning.Algorithms.HybridAStar.Heuristics
{
    internal sealed class GridShortestPathHeuristic : IHeuristic
    {
        private readonly ShortestPathNode shortestPathStart;
        private readonly ITrack raceTrack;
        private readonly double maxSpeed;
        private readonly BoundingSphereCollisionDetector collisionDetector;

        public GridShortestPathHeuristic(
            Vector startPosition,
            IReadOnlyList<IGoal> wayPoints,
            ITrack raceTrack,
            BoundingSphereCollisionDetector collisionDetector,
            double stepSize,
            double maxSpeed)
        {
            this.raceTrack = raceTrack;
            this.maxSpeed = maxSpeed;
            this.collisionDetector = collisionDetector;

            var path = findShortestPath(startPosition, wayPoints, stepSize);
            if (path == null)
            {
                throw new Exception($"Cannot find path from [{startPosition.X}, {startPosition.Y}] to goal.");
            }

            shortestPathStart = simplifyPath(path);
        }

        public TimeSpan EstimateTimeToGoal(VehicleState state, int nextWayPoint)
        {
            // find next directly visible node on the track
            var node = furthestNodeDirectlyVisibleFrom(state.Position, nextWayPoint);
            if (node == null)
            {
                // noooo! can this even happen?! I don't think so.
                return shortestPathStart.CostToTheGoal;
            }

            // calculate cost to the node
            var distance = (node.Position - state.Position).CalculateLength();
            var minCostToNextNode = TimeSpan.FromSeconds(distance / maxSpeed);

            // add that to the cost to the goal
            return minCostToNextNode + node.CostToTheGoal;
        }

        private ShortestPathNode? findShortestPath(Vector start, IReadOnlyList<IGoal> wayPoints, double stepSize)
        {
            var open = new BinaryHeapOpenSet<GridKey, GridSearchNode>();
            var closed = new ClosedSet<GridKey>();

            open.Add(
                new GridSearchNode(
                    key: new GridKey(start, wayPoints.Count),
                    start,
                    previous: null,
                    distanceFromStart: 0.0,
                    estimatedCost: 0.0,
                    wayPoints,
                    targetWayPoint: 0));

            while (!open.IsEmpty)
            {
                var nodeToExpand = open.DequeueMostPromissing();

                if (nodeToExpand.RemainingWayPoints.Count == 0)
                {
                    var head = new ShortestPathNode(
                        wayPoints.Last().Position, // use the goal position directly
                        targetWayPoint: wayPoints.Count - 1); // the goal way point should be kept here

                    var backtrackingNode = nodeToExpand.Previous;
                    while (backtrackingNode != null)
                    {
                        var node = new ShortestPathNode(backtrackingNode.Position, backtrackingNode.TargetWayPoint);
                        node.CostToNext = TimeSpan.FromSeconds(Distance.Between(node.Position, head.Position) / maxSpeed);
                        node.Next = head;
                        head = node;
                        backtrackingNode = backtrackingNode.Previous;
                    }

                    return head;
                }

                closed.Add(nodeToExpand.Key);

                for (var dx = -1; dx <= 1; dx++)
                {
                    for (var dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;

                        var nextPoint = new Vector(
                            nodeToExpand.Position.X + dx * stepSize,
                            nodeToExpand.Position.Y + dy * stepSize);

                        var reachedWayPoint = nodeToExpand.RemainingWayPoints[0].ReachedGoal(nextPoint);
                        var remainingWayPoints = reachedWayPoint
                            ? nodeToExpand.RemainingWayPoints.Skip(1).ToList().AsReadOnly()
                            : nodeToExpand.RemainingWayPoints;

                        var targetWayPoint = wayPoints.Count - nodeToExpand.RemainingWayPoints.Count; // I want to keep the ID of the waypoint in the node which reaches the waypoint and only increase it for its childer

                        var key = new GridKey(nextPoint, remainingWayPoints.Count);
                        if (closed.Contains(key))
                        {
                            continue;
                        }

                        if (collisionDetector.IsCollision(nextPoint))
                        {
                            closed.Add(key);
                            continue;
                        }

                        var distance = nodeToExpand.DistanceFromStart + (nextPoint - nodeToExpand.Position).CalculateLength();
                        var node = new GridSearchNode(key, nextPoint, nodeToExpand, distance, distance + 0, remainingWayPoints, targetWayPoint);
                        if (open.Contains(node.Key))
                        {
                            if (node.DistanceFromStart < open.Get(node.Key).DistanceFromStart)
                            {
                                open.ReplaceExistingWithTheSameKey(node);
                            }
                        }
                        else
                        {
                            open.Add(node);
                        }
                    }
                }
            }

            return null;
        }

        private ShortestPathNode simplifyPath(ShortestPathNode head)
        {
            var start = head;
            var node = head.Next;

            while (node != null)
            {
                if (node.IsGoal || node.Next != null && (!areInLineOfSight(start.Position, node.Next.Position) || start.TargetWayPoint != node.TargetWayPoint))
                {
                    start.Next = node;
                    start.CostToNext = TimeSpan.FromSeconds(Distance.Between(start.Position, node.Position) / maxSpeed);
                    start = node;
                }

                node = node.Next;
            }

            return head;
        }

        private ShortestPathNode? furthestNodeDirectlyVisibleFrom(Vector position, int targetWayPoint)
        {
            ShortestPathNode? furthestNodeSoFar = null;
            var candidate = shortestPathStart;

            while (candidate.Next != null)
            {
                if (candidate.TargetWayPoint < targetWayPoint)
                {
                    candidate = candidate.Next;
                    continue;
                }

                if (areInLineOfSight(candidate.Position, position))
                {
                    furthestNodeSoFar = candidate;
                }
                else if (furthestNodeSoFar != null)
                {
                    return furthestNodeSoFar;
                }

                candidate = candidate.Next;
            }

            return candidate; // the goal
        }

        private bool areInLineOfSight(Vector a, Vector b)
        {
            // assumption: a and b are both in free spots

            // a is the one to the left
            (a, b) = a.X <= b.X ? (a, b) : (b, a);
            var dx = b.X - a.X;
            var dy = b.Y - a.Y;

            // check all the left-right crossings
            var firstCrossing = ((int)(a.X / raceTrack.TileSize) + 1) * raceTrack.TileSize;
            for (var x = firstCrossing; x <= b.X; x += raceTrack.TileSize)
            {
                var y = (int)(((x - a.X) / dx * dy + a.Y) / raceTrack.TileSize);
                var right = (int)(x / raceTrack.TileSize);
                if (!raceTrack.OccupancyGrid[right - 1, y] || !raceTrack.OccupancyGrid[right, y])
                {
                    return false;
                }
            }

            // check all the bottom-up crossings
            (a, b) = a.Y <= b.Y ? (a, b) : (b, a);
            dx = b.X - a.X;
            dy = b.Y - a.Y;

            // check all the left-right crossings
            firstCrossing = ((int)(a.Y / raceTrack.TileSize) + 1) * raceTrack.TileSize;
            for (var y = firstCrossing; y <= b.Y; y += raceTrack.TileSize)
            {
                var x = (int)(((y - a.Y) / dy * dx + a.X) / raceTrack.TileSize);
                var top = (int)(y / raceTrack.TileSize);
                if (!raceTrack.OccupancyGrid[x, top - 1] || !raceTrack.OccupancyGrid[x, top])
                {
                    return false;
                }
            }

            return true;
        }

        private sealed class GridSearchNode : ISearchNode<GridKey>, IComparable<GridSearchNode>
        {
            public GridKey Key { get; }
            public double DistanceFromStart { get; }
            public double EstimatedTotalCost { get; }
            public GridSearchNode? Previous { get; }
            public Vector Position { get; }
            public IReadOnlyList<IGoal> RemainingWayPoints { get; }
            public int TargetWayPoint { get; }

            public GridSearchNode(
                GridKey key,
                Vector position,
                GridSearchNode? previous,
                double distanceFromStart,
                double estimatedCost,
                IReadOnlyList<IGoal> remainingWayPoints,
                int targetWayPoint)
            {
                Key = key;
                Position = position;
                Previous = previous;
                DistanceFromStart = distanceFromStart;
                EstimatedTotalCost = estimatedCost;
                RemainingWayPoints = remainingWayPoints;
                TargetWayPoint = targetWayPoint;
            }

            public int CompareTo(GridSearchNode other)
            {
                var diff = EstimatedTotalCost - other.EstimatedTotalCost;
                return diff < 0 ? -1 : (diff == 0 ? 0 : 1);
            }
        }

        private readonly struct GridKey : IEquatable<GridKey>
        {
            public GridKey(Vector position, int remainingWayPoints)
            {
                Position = position;
                RemainingWayPoints = remainingWayPoints;
            }

            public Vector Position { get; }
            public int RemainingWayPoints { get; }

            public override bool Equals(object obj)
                => (obj is GridKey other) && Equals(other);

            public bool Equals(GridKey other)
                => (Position, RemainingWayPoints) == (other.Position, other.RemainingWayPoints);

            public override int GetHashCode()
                => HashCode.Combine(Position, RemainingWayPoints);
        }

        private sealed class ShortestPathNode
        {
            public Vector Position { get; }
            public ShortestPathNode? Next { get; set; }
            public TimeSpan CostToNext { get; set; }
            public int TargetWayPoint { get; }
            public TimeSpan CostToTheGoal
            {
                get
                {
                    if (!costToGoalCache.HasValue)
                    {
                        costToGoalCache = CostToNext + (Next?.CostToTheGoal ?? TimeSpan.Zero);
                    }

                    return costToGoalCache.Value;
                }
            }
            public bool IsGoal => Next == null;

            private TimeSpan? costToGoalCache;

            public ShortestPathNode(Vector position, int targetWayPoint)
            {
                Position = position;
                Next = null;
                TargetWayPoint = targetWayPoint;
            }
        }
    }
}
