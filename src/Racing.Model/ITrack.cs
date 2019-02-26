using Racing.Mathematics;

namespace Racing.Model
{
    public interface ITrack
    {
        double Width { get; }
        double Height { get; }
        double TileSize { get; }
        ICircuit Circuit { get; }
        bool[,] OccupancyGrid { get; }
        bool IsOccupied(double x, double y);
        bool IsOccupied(int tileX, int tileY);
        bool IsOccupied(Vector position);
        (int x, int y) TileOf(Vector position);
        double DistanceToClosestObstacle(Vector position);
    }
}
