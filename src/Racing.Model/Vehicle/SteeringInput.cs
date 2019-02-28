using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Racing.Model.Vehicle
{
    public sealed class SteeringInputs : IActionSet
    {
        private static readonly IAction brake = new SteeringInput(throttle: -1, steering: 0);
        private static readonly IAction fullThrustForward = new SteeringInput(throttle: 1, steering: 0);

        public const double MinAngle = -1;
        public const double MaxAngle = 1;
        public const double MinThrottle = 0;
        public const double MaxThrottle = 1;

        public IReadOnlyList<IAction> AllPossibleActions { get; }
        public IAction Brake => brake;
        public IAction FullThrustForward => fullThrustForward;

        public SteeringInputs(int throttleSteps = 7, int steeringSteps = 15)
        {
            AllPossibleActions = generateInputs(throttleSteps, steeringSteps).ToImmutableList();
        }

        private static IEnumerable<IAction> generateInputs(int throttleSteps, int steeringSteps)
        {
            if (steeringSteps % 2 != 1)
            {
                throw new ArgumentException($"The number of steering levels must be an odd number, {steeringSteps} is even.");
            }

            yield return fullThrustForward;
            yield return brake;

            var steeringShift = steeringSteps / 2;
            for (int s = 0; s < steeringSteps; s++)
            {
                var steering = (double)(s - steeringShift) / steeringShift;
                for (int t = 0; t < throttleSteps; t++)
            {
                    if (t == throttleSteps - 1 && s == steeringShift)
                    {
                        continue; // full thrust forward
                    }

                    yield return new SteeringInput(
                        throttle: (double)t / (throttleSteps - 1),
                        steering);
                }

                if (steering != 0)
                {
                    yield return new SteeringInput(
                        throttle: -1,
                        steering);
                }
            }
        }

        private sealed class SteeringInput : IAction
        {
            public double Throttle { get; }
            public double Steering { get; }

            public SteeringInput(double throttle, double steering)
            {
                Throttle = throttle;
                Steering = steering;
            }

            public override string ToString()
                => $"[t: {Throttle}, s: {Steering}]";
        }
    }
}
