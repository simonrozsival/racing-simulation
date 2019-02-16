using Racing.Agents;
using Racing.CircuitGenerator;
using Racing.Mathematics;
using Racing.Model.Vehicle;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Simulation
{
    internal sealed class Simulation
    {
        private readonly IAgent agent;
        private readonly ITrackDefinition track;
        private readonly IMotionModel motionModel;

        public Simulation(
            IAgent agent,
            ITrackDefinition track,
            IMotionModel motionModel)
        {
            this.agent = agent;
            this.track = track;
            this.motionModel = motionModel;
        }

        public async Task<TimeSpan?> Simulate(TimeSpan simulationStep, TimeSpan perceptionPeriod)
        {
            IState vehicleState = new InitialState(track.Circuit);
            var elapsedTime = TimeSpan.Zero;

            var environmentObservable =
                Observable.Interval(simulationStep)
                    .WithLatestFrom(
                        agent.Actions,
                        (_, action) =>
                        {
                            vehicleState = motionModel.CalculateNextState(vehicleState, action, simulationStep);
                            elapsedTime += simulationStep;
                            return vehicleState;
                        });

            var agentSubscription =
                Observable.Interval(perceptionPeriod)
                    .Select(_ => vehicleState)
                    .Subscribe(agent.Perception);

            var succeeded =
                await environmentObservable
                    .TakeUntil(reachedGoalOrCrashed)
                    .Select(reachedGoal);

            agentSubscription.Dispose();

            return succeeded ? elapsedTime : (TimeSpan?)null;
        }

        private bool reachedGoalOrCrashed(IState state)
            => reachedGoal(state) || crashed(state);

        private bool crashed(IState state) => true;

        private bool reachedGoal(IState state) => false;

        private sealed class InitialState : IState
        {
            public Point Position { get; }
            public Angle HeadingAngle { get; }
            public Angle SteeringAngle => 0;
            public double Velocity => 0;

            public InitialState(ICircuit circuit)
            {
                var startDirection = circuit.WayPoints.Skip(1).First() - circuit.WayPoints.Last();

                Position = circuit.Start;
                HeadingAngle = startDirection.Direction();
            }
        }
    }
}
