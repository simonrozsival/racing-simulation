using Racing.Mathematics;
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
            int inputSamplesCount,
            Length maximumScanningDistance,
            int numberOfSimulationsPerEvaluation)
        {
            this.simulationStep = simulationStep;
            this.perceptionStep = perceptionStep;
            this.maximumSimulationTime = maximumSimulationTime;
            this.world = world;
            this.numberOfSimulationsPerEvaluation = numberOfSimulationsPerEvaluation;

            lidar = new Lidar(world.Track, inputSamplesCount, maximumScanningDistance);
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
                    break;
                case Result.Failed:
                    fitness += 10; // still better than timing out
                    break;
                case Result.Suceeded:
                    fitness += 500; // on top of the "distance travelled" score
                    break;
            }

            //// points for staying alive long (unless it timeouted)
            //if (summary.Result != Result.TimeOut)
            //{
            //    fitness += summary.SimulationTime.TotalSeconds; // this should change later to achieve fast movement
            //}

            // points for every way point passed
            fitness += summary.DistanceTravelled * 1000;

            //foreach (var log in summary.Log)
            //{
            //    // ideas:
            //    // - proportion of time when it was going at max speed
            //    // - minimize number of direction changes
            //    if (log is IActionSelectedEvent selected)
            //    {

            //        if (selected.Action.Throttle == 1
            //            && selected.Action.Steering == 0)
            //        {
            //            fitness += 10.0;
            //        }
            //        else if (selected.Action.Throttle == 1)
            //        {
            //            fitness += 2.5;
            //        }
            //        else if (selected.Action.Throttle > 0.5 && selected.Action.Steering == 1)
            //        {
            //            fitness += 2.5;
            //        }
            //    }
            //}

            return fitness;
        }
    }
}
