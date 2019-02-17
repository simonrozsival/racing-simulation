using Racing.Model;
using Racing.Model.Vehicle;
using System.Collections.Immutable;

namespace Racing.Agents.Algorithms.Planning
{
    internal sealed class PlanningProblem
    {
        public IState InitialState { get; }
        public IVehicleModel VehicleModel { get; }
        public IMotionModel MotionModel { get; }
        public IImmutableList<IAction> PossibleActions { get; }
        public ITrack Environment { get; }
        public IGoal Goal { get; }

        public PlanningProblem(
            IState initialState,
            IVehicleModel vehicleModel,
            IMotionModel motionModel,
            IImmutableList<IAction> actions,
            ITrack environment,
            IGoal goal)
        {
            InitialState = initialState;
            VehicleModel = vehicleModel;
            MotionModel = motionModel;
            PossibleActions = actions;
            Environment = environment;
            Goal = goal;
        }
    }
}
