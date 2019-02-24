namespace Racing.Model
{
    public interface IAgent
    {
        IAction ReactTo(IState state, int waypoint);
    }
}
