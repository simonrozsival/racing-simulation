using System.Collections.Generic;
using Racing.Mathematics;
using Racing.Model;

namespace Racing.IO.Model
{
    internal sealed class Circuit : ICircuit
    {
        public double Radius { get; set; }
        public Point Start { get; set; }
        public Point Goal { get; set; }
        public IList<Point> WayPoints { get; set; }
    }
}
