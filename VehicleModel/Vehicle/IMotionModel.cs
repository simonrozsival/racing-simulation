using System;

namespace Racing.Model.Vehicle
{
    public interface IMotionModel
    {
        IState CalculateNextState(IState state, IAction action, TimeSpan time, IGoal? goal = null);
    }
}