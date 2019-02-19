using Racing.Mathematics;
using Racing.Model.Vehicle;
using System;
using System.Collections.Generic;

namespace Racing.Model.CollisionDetection
{
    public sealed class AccurateCollisionDetector : ICollisionDetector
    {
        private readonly ITrack track;
        private readonly BoundsDetector boundsDetector;
        private readonly double ux;
        private readonly double uy;

        private static readonly double discretizationStep = 0.05;
        private readonly Dictionary<int, Point> frontLeft = new Dictionary<int, Point>();
        private readonly Dictionary<int, Point> front = new Dictionary<int, Point>();
        private readonly Dictionary<int, Point> frontRight = new Dictionary<int, Point>();

        public AccurateCollisionDetector(ITrack track, IVehicleModel vehicleModel, double safetyMargin = 0)
        {
            this.track = track;

            ux = vehicleModel.Length / 2 + safetyMargin;
            uy = vehicleModel.Width / 2 + safetyMargin;

            boundsDetector = new BoundsDetector(track);

            for (var i = 0; i < 2 * Math.PI  / discretizationStep; i++)
            {
                var a = i * discretizationStep;
                var pointA = new Point(ux, -uy).Rotate(a);
                var pointB = new Point(ux, 0).Rotate(a);
                var pointC = new Point(ux, uy).Rotate(a);
                frontLeft.Add(i, pointA);
                front.Add(i, pointB);
                frontRight.Add(i, pointC);
            }
        }

        public bool IsCollision(IState state)
        {
            var i = discretize(state.HeadingAngle.Radians);
            var pointA = state.Position + frontLeft[i];
            if (isCollision(pointA))
            {
                return true;
            }

            var pointB = state.Position + front[i];
            if (isCollision(pointA))
            {
                return true;
            }

            var pointC = state.Position + frontRight[i];
            return isCollision(pointB);
        }

        private bool isCollision(Point point)
        {
            if (boundsDetector.IsOutOfBounds(point.X, point.Y))
            {
                return true;
            }

            var tileX = (int)(point.X / track.TileSize);
            var tileY = (int)(point.Y / track.TileSize);

            if (!track.OccupancyGrid[tileX, tileY])
            {
                return true;
            }

            return false;
        }

        private static int discretize(double angle)
        {
            while (angle < 0) angle += 2 * Math.PI;
            while (angle >= 2 * Math.PI) angle -= 2 * Math.PI;
            return (int)(angle / discretizationStep);
        }
    }
}
