using Racing.Agents;
using Racing.Mathematics;
using Racing.Model;
using Racing.Model.Vehicle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Racing.Simulation
{
    internal sealed class Simulation
    {
        private readonly IAgent agent;
        private readonly ITrack track;
        private readonly IStateClassificator stateClassificator;
        private readonly IMotionModel motionModel;

        public Simulation(
            IAgent agent,
            ITrack track,
            IStateClassificator stateClassificator,
            IMotionModel motionModel)
        {
            this.agent = agent;
            this.track = track;
            this.stateClassificator = stateClassificator;
            this.motionModel = motionModel;
        }

        public async Task<SimulationResult> Simulate(TimeSpan simulationStep, TimeSpan perceptionStep)
        {
            var states = new List<Log<IState>>();
            var actions = new List<Log<IAction>>();

            IState vehicleState = new InitialState(track.Circuit);
            IAction nextAction = new NoOpAction();

            var elapsedTime = TimeSpan.Zero;
            var timeToNextPerception = TimeSpan.Zero;

            var agentActionsSubscription = agent.Actions
                .Subscribe(action =>
                {
                    actions.Add(new Log<IAction>(elapsedTime, action));
                    nextAction = action;
                });

            var stateType = stateClassificator.Classify(vehicleState);

            while (stateType == StateType.Free)
            {
                vehicleState = motionModel.CalculateNextState(vehicleState, nextAction, simulationStep);
                states.Add(new Log<IState>(elapsedTime, vehicleState));

                elapsedTime += simulationStep;
                timeToNextPerception -= simulationStep;

                if (timeToNextPerception - simulationStep < TimeSpan.Zero)
                {
                    agent.Perception.OnNext(vehicleState);
                    timeToNextPerception = perceptionStep;
                }

                stateType = stateClassificator.Classify(vehicleState);
            }

            var succeeded = stateClassificator.Classify(vehicleState) == StateType.Goal;
            return new SimulationResult(states, actions, elapsedTime, succeeded);
        }

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

        private sealed class NoOpAction : IAction
        {
            public double Throttle => 0;
            public double Steering => 0;
        }
    }
}
