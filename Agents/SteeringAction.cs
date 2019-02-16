using Racing.Model.Vehicle;
using System.Collections.Immutable;

namespace Racing.Agents
{
    internal sealed class SteeringAction : IAction
    {
        public const double MinAngle = -1;
        public const double MaxAngle = 1;
        public const double MinThrottle = 0;
        public const double MaxThrottle = 1;

        public static ImmutableList<SteeringAction> PossibleActions { get; }

        public double Throttle { get; }
        public double Steering { get; }

        private SteeringAction(double throttle, double steering)
        {
            Throttle = throttle;
            Steering = steering;
        }

        static SteeringAction()
        {
            PossibleActions = new[]
            {
                new SteeringAction(throttle: 0, steering: -1),
                new SteeringAction(throttle: 0, steering: 0),
                new SteeringAction(throttle: 0, steering: 1),
                new SteeringAction(throttle: 1, steering: -1),
                new SteeringAction(throttle: 1, steering: 0),
                new SteeringAction(throttle: 1, steering: 1),
            }.ToImmutableList();
        }
    }
}
