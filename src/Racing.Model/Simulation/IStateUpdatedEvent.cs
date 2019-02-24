namespace Racing.Model.Simulation
{
    public interface IStateUpdatedEvent : IEvent
    {
        IState State { get; }
    }
}
