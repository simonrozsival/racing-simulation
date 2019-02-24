using Racing.Mathematics;
using Racing.Model.Vehicle;
using System;
using System.Collections.Generic;

namespace Racing.Model.CollisionDetection
{
    public sealed class AccurateCollisionDetector : ICollisionDetector
    {
        private readonly ITrack track;
        private readonly double ux;
        private readonly double uy;

        private static readonly double discretizationStep = 0.05;
        private readonly Dictionary<int, Vector> frontLeft = new Dictionary<int, Vector>();
        private readonly Dictionary<int, Vector> front = new Dictionary<int, Vector>();
        private readonly Dictionary<int, Vector> frontRight = new Dictionary<int, Vector>();

        public AccurateCollisionDetector(ITrack track, IVehicleModel vehicleModel, double safetyMargin = 0)
        {
            this.track = track;

            ux = vehicleModel.Length / 2 + safetyMargin;
            uy = vehicleModel.Width / 2 + safetyMargin;

            for (var i = 0; i < 2 * Math.PI  / discretizationStep; i++)
            {
                var a = i * discretizationStep;
                var pointA = new Vector(ux, -uy).Rotate(a);
                var pointB = new Vector(ux, 0).Rotate(a);
                var pointC = new Vector(ux, uy).Rotate(a);
                frontLeft.Add(i, pointA);
                front.Add(i, pointB);
                frontRight.Add(i, pointC);
            }
        }

        public bool IsCollision(IState state)
        {
            var i = discretize(state.HeadingAngle.Radians);
            var pointA = state.Position + frontLeft[i];
            if (isOccupied(pointA))
            {
                return true;
            }

            var pointB = state.Position + front[i];
            if (isOccupied(pointA))
            {
                return true;
            }

            var pointC = state.Position + frontRight[i];
            return isOccupied(pointB);
        }

        private bool isOccupied(Vector point)
        {
            return track.IsOccupied(point.X, point.Y);
        }

        private static int discretize(double angle)
        {
            while (angle < 0) angle += 2 * Math.PI;
            while (angle >= 2 * Math.PI) angle -= 2 * Math.PI;
            return (int)(angle / discretizationStep);
        }
    }
}
