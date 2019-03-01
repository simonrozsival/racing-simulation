namespace Racing.Model.Vehicle
{
    public interface VehicleStateClassificator
    {
        StateType Classify(VehicleState state);
    }
}
