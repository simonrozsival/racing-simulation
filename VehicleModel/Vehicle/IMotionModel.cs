using System;

namespace Racing.Model.Vehicle
{
    public interface IMotionModel
    {
        IState CalculateNextState(IState state, IAction action, TimeSpan simulationTime, out bool collided);
        IState CalculateNextState(IState state, IAction action, TimeSpan simulationTime, IGoal goal, out bool collided, out bool reachedGoal);
    }
}