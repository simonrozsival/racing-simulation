using Racing.Model;

namespace Racing.Agents
{
    public interface IAgent
    {
        IAction ReactTo(IState state);
    }
}
