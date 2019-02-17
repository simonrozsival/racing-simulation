using Racing.Model;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Racing.Agents
{
    internal sealed class SteeringAction : IAction
    {
        public const double MinAngle = -1;
        public const double MaxAngle = 1;
        public const double MinThrottle = 0;
        public const double MaxThrottle = 1;

        public static ImmutableList<IAction> PossibleActions { get; }
        public static IAction Brake { get; } = new SteeringAction(0, 0);

        public double Throttle { get; }
        public double Steering { get; }

        private SteeringAction(double throttle, double steering)
        {
            Throttle = throttle;
            Steering = steering;
        }

        static SteeringAction()
        {
            IEnumerable<IAction> generateActions(int throttleSteps, int steeringSteps)
            {
                for (double t = 0; t <= 1.0; t += 1.0 / throttleSteps)
                {
                    for (double s = 0; s <= 1.0; s += 1.0 / steeringSteps)
                    {
                        yield return new SteeringAction(throttle: t, steering: s);
                        if (s != 0)
                        {
                            yield return new SteeringAction(throttle: t, steering: -s);
                        }
                    }
                }
            }

            PossibleActions = generateActions(throttleSteps: 10, steeringSteps: 10).ToImmutableList();
        }
    }
}
