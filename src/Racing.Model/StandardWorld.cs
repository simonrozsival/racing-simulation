using Racing.Model.CollisionDetection;
using Racing.Model.Vehicle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Racing.Model
{
    public sealed class StandardWorld : IWorldDefinition
    {
        public ITrack Track { get; }
        public IVehicleModel VehicleModel { get; }
        public IMotionModel MotionModel { get; }
        public ICollisionDetector CollisionDetector { get; }
        public VehicleStateClassificator StateClassificator { get; }
        public VehicleState InitialState { get; }
        public IActionSet Actions { get; }
        public IReadOnlyList<IGoal> WayPoints { get; }

        public StandardWorld(ITrack track, TimeSpan simulationTime, double safetyMargin = 1)
        {
            Track = track;
            VehicleModel = new Porsche911VehicleModel(track.Circuit.Radius / 3);
            MotionModel = new KinematicModel(VehicleModel, simulationTime);
            // CollisionDetector = new AccurateCollisionDetector(track, VehicleModel, safetyMargin);
            CollisionDetector = new NoCollisionDetection();
            WayPoints = track.Circuit.WayPoints.Count > 4
                ? new[]
                {
                    track.Circuit.WayPoints[0],
                    track.Circuit.WayPoints.ElementAt(track.Circuit.WayPoints.Count / 3),
                    track.Circuit.WayPoints.ElementAt(2 * track.Circuit.WayPoints.Count / 3),
                    track.Circuit.WayPoints.Last()
                }.ToList().AsReadOnly()
                : track.Circuit.WayPoints.ToList().AsReadOnly();

            StateClassificator = new StateClassificator(CollisionDetector, WayPoints.Last());
            InitialState = track.Circuit.StartingPosition;
            Actions = new SteeringInputs(throttleSteps: 7, steeringSteps: 31);
        }
    }
}
