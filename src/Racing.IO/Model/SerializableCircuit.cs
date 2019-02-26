using Racing.Mathematics;
using System.Collections.Generic;

namespace Racing.IO.Model
{
    internal sealed class SerializableCircuit
    {
        public double Radius { get; set; }
        public Vector Start { get; set; }
        public IList<Vector> WayPoints { get; set; } = new List<Vector>();
    }
}
