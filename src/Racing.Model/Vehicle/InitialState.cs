using Racing.Mathematics;
using System.Linq;

namespace Racing.Model.Vehicle
{
    internal sealed class InitialState : IState
    {
        public Vector Position { get; }
        public double HeadingAngle { get; }
        public double SteeringAngle => 0;
        public double Speed => 0;

        public InitialState(ICircuit circuit)
        {
            var startDirection = circuit.WayPoints.First().Position - circuit.Start;

            Position = circuit.Start;
            HeadingAngle = startDirection.Direction();
        }
    }
}
