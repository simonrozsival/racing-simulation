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
        bool IsOccupied(Vector position);
    }
}
