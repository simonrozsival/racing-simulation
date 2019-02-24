using Racing.Mathematics;
using Racing.Model;

namespace Racing.Planning.Algorithms.HybridAStar
{
    internal sealed partial class StateDiscretizer
    {
        private readonly Length positionXCellSize;
        private readonly Length positionYCellSize;
        private readonly Angle headingAngleCellSize;

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
                headingAngle: (int)(state.HeadingAngle / headingAngleCellSize),
                remainingWayPointsCount);
    }
}
