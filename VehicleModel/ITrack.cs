using Racing.Mathematics;

namespace Racing.Model
{
    public interface ITrack
    {
        Distance Width { get; }
        Distance Height { get; }
        Distance TileSize { get; }
        ICircuit Circuit { get; }
        bool[,] OccupancyGrid { get; }
        bool IsOccupied(double x, double y);
    }
}
