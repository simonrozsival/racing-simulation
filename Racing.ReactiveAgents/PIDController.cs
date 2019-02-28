using System;

namespace Racing.ReactiveAgents
{
    internal sealed class PIDController
    {
        private readonly double kp;
        private readonly double ki;
        private readonly double kd;

        private double previousError = 0.0;
        private double accumulatedError = 0.0;

        public PIDController(double kp, double ki, double kd)
        {
            this.kp = kp;
            this.ki = ki;
            this.kd = kd;
        }

        public double Calculate(double target, double currentValue)
        {
            var error = target - currentValue;
            var errorChange = previousError - error;
            previousError = error;
            accumulatedError += error; // ??? this can become a huge number!

            // virtually zero error - reset the accumulated error
            if (Math.Abs(error) < 1)
            {
                accumulatedError = 0.0;
            }

            return kp * error + ki * accumulatedError + kd * errorChange;
        }
    }
}
