namespace Racing.Model.Vehicle
{
    public interface IStateClassificator
    {
        StateType Classify(IState state);
    }
}
