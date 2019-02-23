using System.Collections.Generic;
using Racing.Mathematics;
using Racing.Model;

namespace Racing.IO.Model
{
    internal sealed class SerializableCircuit
    {
        public double Radius { get; set; }
        public Vector Start { get; set; }
        public IList<Vector> WayPoints { get; set; } = new List<Vector>();
    }
}
