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
    }
}
