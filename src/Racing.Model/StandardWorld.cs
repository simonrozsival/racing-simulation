using Racing.Model.CollisionDetection;
using Racing.Model.Vehicle;
using System;
using System.Linq;

namespace Racing.Model
{
    public sealed class StandardWorld : IWorldDefinition
    {
        public ITrack Track { get; }
        public IVehicleModel VehicleModel { get; }
        public IMotionModel MotionModel { get; }
        public ICollisionDetector CollisionDetector { get; }
        public IStateClassificator StateClassificator { get; }
        public IState InitialState { get; }
        public IActionSet Actions { get; }

        public StandardWorld(ITrack track, TimeSpan simulationTime)
        {
            Track = track;
            VehicleModel = new ForwardDrivingOnlyVehicle(track.Circuit.Radius / 5);
            MotionModel = new DynamicModel(VehicleModel, simulationTime);
            CollisionDetector = new AccurateCollisionDetector(track, VehicleModel, safetyMargin: VehicleModel.Width * 0.2);

            // todo: get rid of this - the circuit way points must be defined correctly
            var allWayPoints = track.Circuit.WayPoints.ToList();
            var wayPoints = allWayPoints.Count > 4
                ? new[] { allWayPoints[0], allWayPoints.ElementAt(allWayPoints.Count / 3), allWayPoints.ElementAt(2 * allWayPoints.Count / 3), allWayPoints.Last() }.ToList()
                : allWayPoints;

            StateClassificator = new StateClassificator(CollisionDetector, wayPoints.Last());

            InitialState = new InitialState(track.Circuit);
            Actions = new SteeringInputs(throttleSteps: 5, steeringSteps: 15);
        }
    }
}
