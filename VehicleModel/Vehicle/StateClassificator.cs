using Racing.Model.CollisionDetection;

namespace Racing.Model.Vehicle
{
    public sealed class StateClassificator : IStateClassificator
    {
        private readonly ICollisionDetector collisionDetector;
        private readonly IGoal goal;

        public StateClassificator(ICollisionDetector collisionDetector, IGoal goal)
        {
            this.collisionDetector = collisionDetector;
            this.goal = goal;
        }

        public StateType Classify(IState state)
        {
            if (collisionDetector.IsCollision(state))
            {
                return StateType.Collision;
            }

            if (goal.ReachedGoal(state.Position))
            {
                return StateType.Goal;
            }

            return StateType.Free;
        }
    }
}
