using System;
using System.Collections.Generic;
using System.Text;

namespace Racing.CircuitGenerator
{
    public interface ITrackDefinition
    {
        ICircuit Circuit { get; }
        bool[,] OccupancyGrid { get; }
    }
}
