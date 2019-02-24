using Racing.Model;
using Racing.Model.Sensing;
using SharpNeat.Phenomes;
using System;
using System.Linq;

namespace Racing.Evolution
{
    public sealed class NeuralNetworkAgent : IAgent
    {
        private readonly ILidar lidar;
        private readonly IBlackBox brain;

        public NeuralNetworkAgent(ILidar lidar, IBlackBox brain)
        {
            if (brain.OutputCount != 2)
            {
                throw new ArgumentException($"The agent needs two prediction outputs.");
            }

            this.lidar = lidar;
            this.brain = brain;
        }

        public IAction ReactTo(IState state, int nextWayPoint)
        {
            var scan = lidar.Scan(state.Position, state.HeadingAngle);
            var distances = scan.Readings.ToList();

            if (distances.Count % brain.InputCount != 0)
            {
                throw new InvalidOperationException($"The neural network expects a multiple of {brain.InputCount} inputs but it was given {distances.Count}.");
            }

            var everyNth = distances.Count / brain.InputCount;
            var readings = distances.Where((_, i) => i % everyNth == 0).Select(distance => distance.Meters).ToArray();

            var prediction = predict(readings);

            return new PredictedAction(prediction);
        }

        private double[] predict(double[] readings)
        {
            brain.ResetState();
            brain.InputSignalArray.CopyFrom(readings, 0);

            brain.Activate();

            var prediction = new double[brain.OutputCount];
            brain.OutputSignalArray.CopyTo(prediction, 0);

            return prediction;
        }

        private sealed class PredictedAction : IAction
        {
            public PredictedAction(double[] prediction)
            {
                Throttle = prediction[0];
                Steering = prediction[1];
            }

            public double Throttle { get; }
            public double Steering { get; }
        }
    }
}
