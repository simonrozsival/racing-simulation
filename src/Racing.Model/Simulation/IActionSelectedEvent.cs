namespace Racing.Model.Simulation
{
    public interface IActionSelectedEvent : IEvent
    {
        IAction Action { get; }
    }
}
