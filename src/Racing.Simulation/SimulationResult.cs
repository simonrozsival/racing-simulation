using Racing.Model.Simulation;
using System;
using System.Collections.Generic;

namespace Racing.Simulation
{
    internal sealed class SimulationSummary : ISummary
    {
        public SimulationSummary(TimeSpan simulationTime, Result result, IEnumerable<IEvent> log)
        {
            SimulationTime = simulationTime;
            Result = result;
            Log = log;
        }

        public TimeSpan SimulationTime { get; }
        public Result Result { get; }
        public IEnumerable<IEvent> Log { get; }
    }
}
