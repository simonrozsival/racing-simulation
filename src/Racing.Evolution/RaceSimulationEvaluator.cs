using Racing.Model;
using Racing.Model.Sensing;
using Racing.Model.Simulation;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using System;

namespace Racing.Evolution
{
    internal sealed class RaceSimulationEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        private readonly TimeSpan simulationStep;
        private readonly TimeSpan perceptionStep;
        private readonly TimeSpan maximumSimulationTime;
        private readonly ILidar lidar;
        private readonly IWorldDefinition world;
        private readonly int numberOfSimulationsPerEvaluation;
        private readonly object evaluationLock = new object();

        public RaceSimulationEvaluator(
            TimeSpan simulationStep,
            TimeSpan perceptionStep,
            TimeSpan maximumSimulationTime,
            IWorldDefinition world,
            ILidar lidar,
            int numberOfSimulationsPerEvaluation)
        {
            this.simulationStep = simulationStep;
            this.perceptionStep = perceptionStep;
            this.maximumSimulationTime = maximumSimulationTime;
            this.world = world;
            this.numberOfSimulationsPerEvaluation = numberOfSimulationsPerEvaluation;
            this.lidar = lidar;
        }

        public ulong EvaluationCount { get; private set; } = 0;

        public bool StopConditionSatisfied => false;

        public FitnessInfo Evaluate(IBlackBox phenome)
        {
            var agent = new NeuralNetworkAgent(lidar, phenome);
            var simulation = new Simulation.Simulation(agent, world);

            var fitness = 0.0;

            for (var i = 0; i < numberOfSimulationsPerEvaluation; i++)
            {
                var simulationResult = simulation.Simulate(simulationStep, perceptionStep, maximumSimulationTime);
                fitness += fitnessOf(simulationResult);
            }

            lock (evaluationLock)
            {
                EvaluationCount++;
            }

            return new FitnessInfo(
                fitness: fitness / numberOfSimulationsPerEvaluation,
                new AuxFitnessInfo[0]);
        }

        public void Reset()
        {
            lock (evaluationLock)
            {
                EvaluationCount = 0;
            }
        }

        private double fitnessOf(ISummary summary)
        {
            var fitness = 0.0;

            // time-out x crashing x nothing
            switch (summary.Result)
            {
                case Result.TimeOut:
                    return 0.0;
                case Result.Failed:
                    // still better than timing out
                    break;
                case Result.Suceeded:
                    fitness += 100; // on top of the "distance travelled" score
                    break;
            }

            // points for every way point passed
            fitness += summary.DistanceTravelled * 100 / Math.Max(1.0, summary.SimulationTime.TotalSeconds);

            var actionsInTotal = 0;
            var actionsWithHightThrottleAndSmallSteeringAngle = 0;

            foreach (var log in summary.Log)
            {
                // ideas:
                // - proportion of time when it was going at max speed
                // - minimize number of direction changes
                if (log is IActionSelectedEvent selected)
                {
                    actionsInTotal++;

                    if (selected.Action.Throttle > 0.75 && Math.Abs(selected.Action.Steering) < 0.2)
                    {
                        actionsWithHightThrottleAndSmallSteeringAngle++;
                    }
                }
            }

            fitness += summary.DistanceTravelled * 5 * ((double)actionsWithHightThrottleAndSmallSteeringAngle / actionsInTotal);

            return Math.Max(0.0, fitness);
        }
    }
}
