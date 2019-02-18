using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Racing.Model.Vehicle
{
    public sealed class SteeringInput : IAction
    {
        public const double MinAngle = -1;
        public const double MaxAngle = 1;
        public const double MinThrottle = 0;
        public const double MaxThrottle = 1;

        public static ImmutableList<IAction> PossibleActions { get; }
        public static SteeringInput Brake { get; } = new SteeringInput(0, 0);

        public double Throttle { get; }
        public double Steering { get; }

        private SteeringInput(double throttle, double steering)
        {
            Throttle = throttle;
            Steering = steering;
        }

        static SteeringInput()
        {
            PossibleActions = generateInputs(9, 33).ToImmutableList();
        }

        public override string ToString()
            => $"[t: {Throttle}, s: {Steering}]";

        private static IEnumerable<IAction> generateInputs(int throttleSteps, int steeringSteps)
        {
            if (steeringSteps % 2 != 1)
            {
                throw new ArgumentException($"The number of steering levels must be an odd number, {steeringSteps} is even.");
            }

            var steeringShift = steeringSteps / 2;
            for (int t = 0; t < throttleSteps; t++)
            {
                for (int s = 0; s < steeringSteps; s++)
                {
                    yield return new SteeringInput(
                        throttle: (double)t / (throttleSteps - 1),
                        steering: (double)(s - steeringShift) / steeringShift);
                }
            }
        }
    }
}
