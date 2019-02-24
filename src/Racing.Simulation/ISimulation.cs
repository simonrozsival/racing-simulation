using System;
using System.Threading.Tasks;
using Racing.Model.Simulation;

namespace Racing.Simulation
{
    public interface ISimulation
    {
        IObservable<IEvent> Events { get; }
        ISummary Simulate(TimeSpan simulationStep, TimeSpan perceptionStep, TimeSpan maximumSimulationTime);
    }
}
