using Racing.Mathematics;

namespace Racing.Model
{
    public interface ITrack
    {
        Length Width { get; }
        Length Height { get; }
        Length TileSize { get; }
        ICircuit Circuit { get; }
        bool[,] OccupancyGrid { get; }
        bool IsOccupied(Length x, Length y);
        bool IsOccupied(int tileX, int tileY);
        bool IsOccupied(Vector position);
        (int x, int y) TileOf(Vector position);
        Length DistanceToClosestObstacle(Vector position);
    }
}
