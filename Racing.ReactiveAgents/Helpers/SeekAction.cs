﻿using Racing.Model;

namespace Racing.ReactiveAgents.Helpers
{
    internal sealed class SeekAction : IAction
    {
        public SeekAction(double throttle, double steering)
        {
            Throttle = throttle;
            Steering = steering;
        }

        public double Throttle { get; }
        public double Steering { get; }
    }
}
