using Racing.Model.Simulation;
using System;
using System.Collections.Generic;

namespace Racing.Simulation
{
    public sealed class SimulationSummary : ISummary
    {
        public SimulationSummary(
            TimeSpan simulationTime,
            Result result,
            IEnumerable<IEvent> log,
            double distanceTravelled)
        {
            SimulationTime = simulationTime;
            Result = result;
            Log = log;
            DistanceTravelled = distanceTravelled;
        }

        public TimeSpan SimulationTime { get; }
        public Result Result { get; }
        public IEnumerable<IEvent> Log { get; }
        public double DistanceTravelled { get; }
    }
}
