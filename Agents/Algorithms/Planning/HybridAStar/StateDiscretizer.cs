using Racing.Model;

namespace Racing.Agents.Algorithms.Planning.HybridAStar
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

        public DiscreteState Discretize(IState state, int remainingWayPointsCount)
            => new DiscreteState(
                x: (int)(state.Position.X / positionXCellSize),
                y: (int)(state.Position.Y / positionYCellSize),
                headingAngle: (int)(state.HeadingAngle.Radians / headingAngleCellSize),
                remainingWayPointsCount);
    }
}
