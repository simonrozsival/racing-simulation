namespace Racing.Model.Simulation
{
    public interface ISimulationEndedEvent : IEvent
    {
        Result Result { get; }
    }
}
