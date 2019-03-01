using Racing.Model.Vehicle;

namespace Racing.Model.Simulation
{
    public interface VehicleStateUpdatedEvent : IEvent
    {
        VehicleState State { get; }
    }
}
