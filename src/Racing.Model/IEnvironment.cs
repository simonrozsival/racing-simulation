using Racing.Model.CollisionDetection;
using Racing.Model.Vehicle;
using System.Collections.Generic;

namespace Racing.Model
{
    public interface IWorldDefinition
    {
        ITrack Track { get; }
        IReadOnlyList<IGoal> WayPoints { get; }
        IVehicleModel VehicleModel { get; }
        IMotionModel MotionModel { get; }
        ICollisionDetector CollisionDetector { get; }
        IStateClassificator StateClassificator { get; }
        IState InitialState { get; }
        IActionSet Actions { get; }
    }
}
