using Racing.Model.Vehicle;

namespace Racing.Model
{
    public interface IGoal
    {
        bool ReachedGoal(IState state);
    }
}
