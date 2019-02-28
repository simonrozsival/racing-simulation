using Racing.Model.Visualization;

namespace Racing.Model.Simulation
{
    public interface IVisualizationEvent : IEvent
    {
        IVisualization Visualization { get; }
    }
}
