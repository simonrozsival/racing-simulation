using Racing.CircuitGenerator.Splines;
using System.Collections.Generic;

namespace Racing.CircuitGenerator
{
    public interface ICircuit
    {
        double Radius { get; }
        Point Start { get; }
        Point Goal { get; }
        IList<Point> WayPoints { get; }
    }
}
