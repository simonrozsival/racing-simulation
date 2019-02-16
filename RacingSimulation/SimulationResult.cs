using Racing.Model;
using System;
using System.Collections.Generic;

namespace Racing.Simulation
{
    internal sealed class SimulationResult
    {
        public IList<Log<IState>> States { get; }
        public IList<Log<IAction>> Actions { get; }
        public TimeSpan SimulationTime { get; }
        public bool Succeeded { get; }

        public SimulationResult(
            IList<Log<IState>> states,
            IList<Log<IAction>> actions,
            TimeSpan simulationTime,
            bool succeeded)
        {
            States = states;
            Actions = actions;
            SimulationTime = simulationTime;
            Succeeded = succeeded;
        }
    }
}
