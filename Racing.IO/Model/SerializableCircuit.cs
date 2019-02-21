using System.Collections.Generic;
using Racing.Mathematics;
using Racing.Model;

namespace Racing.IO.Model
{
    internal sealed class SerializableCircuit
    {
        public double Radius { get; set; }
        public Point Start { get; set; }
        public IList<Point> WayPoints { get; set; } = new List<Point>();
    }
}
