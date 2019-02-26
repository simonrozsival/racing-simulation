using Racing.Mathematics;
using System;
using System.Collections.Generic;

namespace Racing.Model
{
    public sealed class RaceTrack : ITrack
    {
        public Length TileSize { get; }
        public ICircuit Circuit { get; }
        public bool[,] OccupancyGrid { get; }
        public Length Width { get; }
        public Length Height { get; }

        public RaceTrack(
            ICircuit circuit,
            bool[,] occupancyGrid,
            Length tileSize)
        {
            Circuit = circuit;
            OccupancyGrid = occupancyGrid;
            TileSize = tileSize;
            Width = OccupancyGrid.GetLength(0) * tileSize;
            Height = OccupancyGrid.GetLength(1) * tileSize;
        }

        public bool IsOccupied(Vector position)
            => IsOccupied(position.X, position.Y);

        public bool IsOccupied(Length x, Length y)
        {
            var (tileX, tileY) = tileOf(x, y);
            return IsOccupied(tileX, tileY);
        }

        public bool IsOccupied(int x, int y)
        {
            if (x < 0
                || y < 0
                || x >= OccupancyGrid.GetLength(0)
                || y >= OccupancyGrid.GetLength(1))
            {
                return true;
            }

            return !OccupancyGrid[x, y];
        }

        public (int x, int y) TileOf(Vector point)
            => tileOf(point.X, point.Y);

        private (int x, int y) tileOf(Length x, Length y)
        {
            var tileX = (int)(x / TileSize).Meters;
            var tileY = (int)(y / TileSize).Meters;

            return (tileX, tileY);
        }

        public Length DistanceToClosestObstacle(Vector position)
        {
            var distances = new int[OccupancyGrid.GetLength(0), OccupancyGrid.GetLength(1)];
            for (var i = 0; i < distances.GetLength(0); i++)
            {
                for (var j = 0; j < distances.GetLength(1); j++)
                {
                    distances[i, j] = int.MinValue;
                }
            }

            var stack = new Stack<(int x, int y)>();

            var startTile = TileOf(position);
            stack.Push(startTile);
            distances[startTile.x, startTile.y] = 0;

            while (stack.Count > 0)
            {
                var tile = stack.Pop();
                var distance = distances[tile.x, tile.y] + 1;

                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        var x = tile.x + j;
                        var y = tile.y + j;
                        if (x < 0 || y < 0 || x >= distances.GetLength(0) || y >= distances.GetLength(1))
                        {
                            return distance;
                        }

                        if (distances[x, y] == int.MinValue)
                        {
                            if (IsOccupied(x, y))
                            {
                                return distance;
                            }

                            distances[x, y] = distance;
                            stack.Push((x, y));
                        }
                    }
                }
            }

            throw new Exception($"This cannot happen.");
        }
    }
}
