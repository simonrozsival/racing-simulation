using Racing.Mathematics;
using System.Collections.Generic;

namespace Racing.Model
{
    public interface ICircuit
    {
        double Radius { get; }
        Point Start { get; }
        Point Goal { get; }
        IList<Point> WayPoints { get; }
    }
}
