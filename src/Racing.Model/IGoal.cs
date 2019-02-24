using Racing.Mathematics;

namespace Racing.Model
{
    public interface IGoal
    {
        Vector Position { get; }
        bool ReachedGoal(Vector position);
    }
}
