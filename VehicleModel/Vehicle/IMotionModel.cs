using System;

namespace RacePlanning.Model.VehicleModel
{
    internal interface IMotionModel
    {
        VehicleState CalculateNextState(VehicleState state, SteeringInput action, double time);
    }
}
