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
        private readonly Func<ITrack, ILidar> createLidarFor;
        private readonly IWorldDefinition[] worlds;
        private readonly object evaluationLock = new object();

        public RaceSimulationEvaluator(
            Random random,
            TimeSpan simulationStep,
            TimeSpan perceptionStep,
            TimeSpan maximumSimulationTime,
            IWorldDefinition[] worlds,
            Func<ITrack, ILidar> createLidarFor)
        {
            this.simulationStep = simulationStep;
            this.perceptionStep = perceptionStep;
            this.maximumSimulationTime = maximumSimulationTime;
            this.worlds = worlds;
            this.createLidarFor = createLidarFor;
        }

        public ulong EvaluationCount { get; private set; } = 0;

        public bool StopConditionSatisfied => false;

        public FitnessInfo Evaluate(IBlackBox phenome)
        {
            var fitness = 0.0;

            foreach (var world in worlds)
            {
                var lidar = createLidarFor(world.Track);
                var agent = new NeuralNetworkAgent(lidar, phenome);
                var simulation = new Simulation.Simulation(agent, world);

                var simulationResult = simulation.Simulate(simulationStep, perceptionStep, maximumSimulationTime);
                fitness += fitnessOf(simulationResult);
            }

            lock (evaluationLock)
            {
                EvaluationCount++;
            }

            return new FitnessInfo(
                fitness: fitness / worlds.Length,
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

            var totalDistanceTravelled = 0.0;
            IState? previous = null;

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
                else if (log is IStateUpdatedEvent updated)
                {
                    if (previous != null)
                    {
                        totalDistanceTravelled += Distance.Between(updated.State.Position, previous.Position);
                    }

                    previous = updated.State;
                }
            }

            fitness += 10 * ((double)actionsWithHightThrottleAndSmallSteeringAngle / actionsInTotal);
            fitness += 10 * summary.SimulationTime.TotalSeconds / totalDistanceTravelled;

            return Math.Max(0.0, fitness);
        }
    }
}
