using Racing.Mathematics;
using System.Collections.Generic;

namespace Racing.Model
{
    public interface ICircuit
    {
        double Radius { get; }
        Vector Start { get; }
        IList<IGoal> WayPoints { get; }
    }
}
