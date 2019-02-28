using Racing.Mathematics;
using Racing.Model;

namespace Racing.IO.Model
{
    internal sealed class SerializableState : IState
    {
        public Vector Position { get; set; }

        public double HeadingAngle { get; set; }

        public double SteeringAngle { get; set; }

        public double Speed { get; set; }
    }
}
