using System;
using System.Collections.Generic;

namespace Racing.Model.Simulation
{
    public interface ISummary
    {
        TimeSpan SimulationTime { get; }
        double DistanceTravelled { get; }
        Result Result { get; }
        IEnumerable<IEvent> Log { get; }
    }
}
