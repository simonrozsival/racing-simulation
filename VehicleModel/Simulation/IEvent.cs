using System;

namespace Racing.Model.Simulation
{
    public interface IEvent
    {
        TimeSpan Time { get; }
    }
}
