using System;

namespace Racing.Simulation
{
    internal sealed class Log<T>
    {
        public TimeSpan Time { get; }
        public T Value { get; }

        public Log(TimeSpan time, T value)
        {
            Time = time;
            Value = value;
        }
    }
}
