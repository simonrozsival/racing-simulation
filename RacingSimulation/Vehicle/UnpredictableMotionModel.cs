using Racing.Mathematics;
using Racing.Model;
using Racing.Model.Vehicle;
using System;

namespace Racing.Simulation.Vehicle
{
    internal sealed class UnpredictableMotionModel : IMotionModel
    {
        private readonly IMotionModel internalModel;
        private readonly double bias;
        private readonly Random random;

        public UnpredictableMotionModel(
            IMotionModel internalModel,
            double bias,
            Random random)
        {
            this.internalModel = internalModel;
            this.bias = bias;
            this.random = random;
        }

        public IState CalculateNextState(IState state, IAction action, TimeSpan time)
        {
            var accurateState = internalModel.CalculateNextState(state, action, time);
            return new NoisyState(accurateState, bias, random);
        }

        private sealed class NoisyState : IState
        {
            public Point Position { get; }
            public Angle HeadingAngle { get; }
            public Angle SteeringAngle { get; }
            public double Velocity { get; }

            public NoisyState(IState state, double bias, Random random)
            {
                Position = state.Position + random.Next(-1, 1) * new Point(bias * random.NextDouble(), bias * random.NextDouble());
                HeadingAngle = state.HeadingAngle + bias * random.NextDouble() * random.Next(-1, 1);
                SteeringAngle = state.HeadingAngle + bias * random.NextDouble() * random.Next(-1, 1);
            }
        }
    }
}
