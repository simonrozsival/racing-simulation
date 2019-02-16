namespace Racing.Model.Vehicle
{
    internal interface IMotionModel
    {
        IState CalculateNextState(IState state, IAction action, double time);
    }
}