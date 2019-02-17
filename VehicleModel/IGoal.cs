using Racing.Mathematics;

namespace Racing.Model
{
    public interface IGoal
    {
        Point Position { get; }
        bool ReachedGoal(Point position);
    }
}
