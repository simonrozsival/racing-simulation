using Racing.Model.CollisionDetection;
using Racing.Model.Vehicle;
using System;
using System.Collections.Generic;
using System.Text;

namespace Racing.Model
{
    public interface IWorldDefinition
    {
        ITrack Track { get; }
        IVehicleModel VehicleModel { get; }
        IMotionModel MotionModel { get; }
        ICollisionDetector CollisionDetector { get; }
        IStateClassificator StateClassificator { get; }
        IState InitialState { get; }
        IActionSet Actions { get; }
    }
}
