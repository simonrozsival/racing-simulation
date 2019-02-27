using Racing.Model;
using Racing.Model.Simulation;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Racing.Simulation
{
    public sealed class Log
    {
        private readonly List<IEvent> history = new List<IEvent>();
        private readonly ISubject<IEvent> events = new Subject<IEvent>();

        private TimeSpan simulationTime = TimeSpan.Zero;

        public IEnumerable<IEvent> History => history;
        public IObservable<IEvent> Events => events.AsObservable();

        public void SimulationTimeChanged(TimeSpan time)
        {
            simulationTime = time;
        }

        public void ActionSelected(IAction action)
        {
            log(new ActionSelectedEvent(action, simulationTime));
        }

        public void StateUpdated(IState state)
        {
            log(new StateUpdatedEvent(state, simulationTime));
        }

        public void Finished(Result result)
        {
            log(new SimulationEndedEvent(result, simulationTime));
            events.OnCompleted();
        }

        private void log(IEvent loggedEvent)
        {
            events.OnNext(loggedEvent);
            history.Add(loggedEvent);
        }

        private sealed class ActionSelectedEvent : IActionSelectedEvent
        {
            public ActionSelectedEvent(IAction action, TimeSpan time)
            {
                Action = action;
                Time = time;
            }

            public IAction Action { get; }

            public TimeSpan Time { get; }
        }

        private sealed class SimulationEndedEvent : ISimulationEndedEvent
        {
            public SimulationEndedEvent(Result result, TimeSpan time)
            {
                Result = result;
                Time = time;
            }

            public Result Result { get; }

            public TimeSpan Time { get; }
        }


        private sealed class StateUpdatedEvent : IStateUpdatedEvent
        {
            public StateUpdatedEvent(IState state, TimeSpan time)
            {
                State = state;
                Time = time;
            }

            public IState State { get; }

            public TimeSpan Time { get; }
        }
    }
}
