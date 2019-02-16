using System.Collections.Generic;
using System.Collections.Immutable;

namespace RacePlanning.Model
{
    internal sealed class SteeringInput
    {
        public const double MinAngle = -1;
        public const double MaxAngle = 1;
        public const double MinThrottle = 0;
        public const double MaxThrottle = 1;

        public static ImmutableList<SteeringInput> PossibleActions { get; }

        public double Throttle { get; }
        public double Steering { get; }

        private SteeringInput(double throttle, double steering)
        {
            Throttle = throttle;
            Steering = steering;
        }

        static SteeringInput()
        {
            PossibleActions = new[]
            {
                new SteeringInput(throttle: 0, steering: -1),
                new SteeringInput(throttle: 0, steering: 0),
                new SteeringInput(throttle: 0, steering: 1),
                new SteeringInput(throttle: 1, steering: -1),
                new SteeringInput(throttle: 1, steering: 0),
                new SteeringInput(throttle: 1, steering: 1),
            }.ToImmutableList();
        }
    }
}
