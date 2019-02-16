using Racing.Mathematics;

namespace Racing.Model
{
    public interface ITrack
    {
        double TileSize { get; }
        ICircuit Circuit { get; }
        bool[,] OccupancyGrid { get; }
    }
}
