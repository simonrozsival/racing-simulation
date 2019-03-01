﻿using Racing.Mathematics;
using Racing.Model;
using Racing.Model.Vehicle;

namespace Racing.Planning.Algorithms.HybridAStar
{
    internal sealed partial class StateDiscretizer
    {
        private readonly double positionXCellSize;
        private readonly double positionYCellSize;
        private readonly double headingAngleCellSize;

        public StateDiscretizer(
            double positionXCellSize,
            double positionYCellSize,
            double headingAngleCellSize)
        {
            this.positionXCellSize = positionXCellSize;
            this.positionYCellSize = positionYCellSize;
            this.headingAngleCellSize = headingAngleCellSize;
        }

        public DiscreteState Discretize(VehicleState state, int targetWayPoint)
            => new DiscreteState(
                x: (int)(state.Position.X / positionXCellSize),
                y: (int)(state.Position.Y / positionYCellSize),
                headingAngle: (int)(state.HeadingAngle / headingAngleCellSize),
                targetWayPoint);
    }
}
