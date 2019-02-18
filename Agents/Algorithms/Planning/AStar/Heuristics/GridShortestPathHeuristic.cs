using Racing.Agents.Algorithms.Planning.AStar.DataStructures;
using Racing.Model;
using Racing.Mathematics;
using System;
using Racing.Model.CollisionDetection;

namespace Racing.Agents.Algorithms.Planning.AStar.Heuristics
{
    internal sealed class GridShortestPathHeuristic : IHeuristic
    {
        private readonly ShortestPathNode shortestPathStart;
        private readonly ITrack raceTrack;
        private readonly double maxSpeed;
        private readonly BoundingSphereCollisionDetector collisionDetector;

        public GridShortestPathHeuristic(
            Point startPosition,
            IGoal goal,
            ITrack raceTrack,
            BoundingSphereCollisionDetector collisionDetector,
            double stepSize,
            double maxSpeed)
        {
            this.raceTrack = raceTrack;
            this.maxSpeed = maxSpeed;
            this.collisionDetector = collisionDetector;

            var path = findShortestPath(startPosition, goal, stepSize);
            if (path == null)
            {
                throw new Exception($"Cannot find path from [{startPosition.X}, {startPosition.Y}] to goal.");
            }

            shortestPathStart = simplifyPath(path);
        }

        public TimeSpan EstimateTimeToGoal(IState state, PlanningProblem problem)
        {
            // find next directly visible node on the track
            var node = furthestNodeDirectlyVisibleFrom(state.Position);

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

        private ShortestPathNode findShortestPath(Point start, IGoal goal, double stepSize)
        {
            var open = new BinaryHeapOpenSet<GridSearchNode>();
            var closed = new ClosedSet<long>();

            open.Add(new GridSearchNode(start, null, 0.0, 0.0));

            while (!open.IsEmpty)
            {
                var nodeToExpand = open.DequeueMostPromissing();

                if (goal.ReachedGoal(nodeToExpand.Position))
                {
                    var head = new ShortestPathNode(nodeToExpand.Position);
                    var backtrackingNode = nodeToExpand.Previous;
                    while (backtrackingNode != null)
                    {
                        var node = new ShortestPathNode(backtrackingNode.Position);
                        node.CostToNext = TimeSpan.FromSeconds((node.Position - head.Position).CalculateLength() / maxSpeed);
                        node.Next = head;
                        head = node;
                        backtrackingNode = backtrackingNode.Previous;
                    }

                    return head;
                }

                closed.Add(nodeToExpand.Id);

                for (var dx = -1; dx <= 1; dx++)
                {
                    for (var dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;

                        var nextPoint = new Point(
                            nodeToExpand.Position.X + dx * stepSize,
                            nodeToExpand.Position.Y + dy * stepSize);

                        var id = hash(nextPoint);
                        if (closed.Contains(id))
                        {
                            continue;
                        }

                        if (collisionDetector.IsCollision(nextPoint))
                        {
                            closed.Add(id);
                            continue;
                        }

                        var distance = nodeToExpand.DistanceFromStart + (nextPoint - nodeToExpand.Position).CalculateLength();
                        var node = new GridSearchNode(nextPoint, nodeToExpand, distance, distance);
                        if (open.Contains(node))
                        {
                            if (node.DistanceFromStart < open.NodeSimilarTo(node).DistanceFromStart)
                            {
                                open.Replace(node);
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
                if (node.IsGoal || !areInLineOfSight(start.Position, node.Next.Position))
                {
                    start.Next = node;
                    start.CostToNext = TimeSpan.FromSeconds((start.Position - node.Position).CalculateLength() / maxSpeed);
                    start = node;
                }

                node = node.Next;
            }

            return head;
        }

        private ShortestPathNode furthestNodeDirectlyVisibleFrom(Point position)
        {
            ShortestPathNode furthestNodeSoFar = null;
            var candidate = shortestPathStart;
            while (candidate.Next != null)
            {
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

            return candidate;
        }

        private bool areInLineOfSight(Point a, Point b)
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

        private sealed class GridSearchNode : ISearchNode, IComparable<GridSearchNode>
        {
            public long Id { get; }
            public double DistanceFromStart { get; }
            public double EstimatedCost { get; }
            public GridSearchNode Previous { get; }
            public Point Position { get; }

            public GridSearchNode(Point position, GridSearchNode previous, double distanceFromStart, double estimatedCost)
            {
                Id = hash(position);
                Position = position;
                Previous = previous;
                DistanceFromStart = distanceFromStart;
                EstimatedCost = estimatedCost;
            }

            public int CompareTo(GridSearchNode other)
            {
                var diff = EstimatedCost - other.EstimatedCost;
                return diff < 0 ? -1 : (diff == 0 ? 0 : 1);
            }
        }

        private sealed class ShortestPathNode
        {
            public Point Position { get; }
            public ShortestPathNode Next { get; set; }
            public TimeSpan CostToNext { get; set; }
            public TimeSpan CostToTheGoal => CostToNext + (Next?.CostToTheGoal ?? TimeSpan.Zero);
            public bool IsGoal => Next == null;

            public ShortestPathNode(Point position)
            {
                Position = position;
            }
        }

        private static long hash(Point position)
            => (long)(position.X) * 10000 + (long)position.Y;
    }
}
