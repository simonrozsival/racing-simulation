using System.Collections.Generic;

namespace Racing.IO.Model
{
    internal sealed class SerializableCircuit
    {
        public double Radius { get; set; }
        public SerializableVector Start { get; set; }
        public IList<SerializableVector> WayPoints { get; set; } = new List<SerializableVector>();
    }
}
