using Racing.Model;

namespace Racing.IO.Model
{
    internal sealed class SerializableAction : IAction
    {
        public double Throttle { get; set; }

        public double Steering { get; set; }
    }
}
