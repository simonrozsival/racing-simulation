using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Racing.Planning.Algorithms.Domain;
using Racing.Model;
using Racing.Model.CollisionDetection;
using Racing.Model.Vehicle;

namespace Racing.Planning.Agents
{
    public class AStarAgent : IAgent
    {
        private readonly TimeSpan perceptionPeriod;
        private readonly TimeSpan simulationStep;
        private readonly Queue<IGoal> pointsToGo;
        private readonly IVehicleModel vehicleModel;
        private readonly IMotionModel motionModel;
        private readonly ITrack track;
        private readonly BoundingSphereCollisionDetector collisionDetector;
        private readonly IActionSet actions;

        private Queue<IAction>? plan = null;
        private IState? previousPercievedState = null;

        public ISubject<IState> ExploredStates { get; } = new Subject<IState>();

        public AStarAgent(
            IVehicleModel vehicleModel,
            IMotionModel motionModel,
            ITrack track,
            IActionSet actions,
            TimeSpan perceptionPeriod)
        {
            this.vehicleModel = vehicleModel;
            this.motionModel = motionModel;
            this.track = track;
            this.actions = actions;
            this.perceptionPeriod = perceptionPeriod;
            this.simulationStep = perceptionPeriod / 10;

            collisionDetector = new BoundingSphereCollisionDetector(track, vehicleModel);

            pointsToGo = new Queue<IGoal>(track.Circuit.WayPoints);
        }

        public IAction ReactTo(IState state)
        {
            if (previousPercievedState != null && pointsToGo.Count > 0)
            {
                var traveledDistance = state.Position - previousPercievedState.Position;
                var steps = (int)Math.Floor(perceptionPeriod / simulationStep);
                for (int i = 0; i < steps; i++)
                {
                    var pointBetween = previousPercievedState.Position + (double)i / steps * traveledDistance;
                    if (pointsToGo.Peek().ReachedGoal(pointBetween))
                    {
                        pointsToGo.Dequeue();
                        plan = createNewPlan(state);
                        break;
                    }
                }
            }

            previousPercievedState = state;

            if (pointsToGo.Count == 0)
            {
                return actions.Brake;
            }

            if (plan == null || plan.Count == 0)
            {
                plan = createNewPlan(state);
            }

            if (plan == null || plan.Count == 0)
            {
                return actions.Brake;
            }

            var nextAction = plan.Dequeue();

            if (nextAction != null && couldLeadToCrash(state, nextAction))
            {
                plan = createNewPlan(state);
                if (plan != null && plan.Count != 0)
                {
                    nextAction = plan.Dequeue();
                }
            }

            // todo: is too off the planned trajectory?

            return nextAction ?? actions.Brake;
        }

        private bool couldLeadToCrash(IState state, IAction action)
        {
            for (int i = 0; i < 3; i++)
            {
                state = motionModel.CalculateNextState(state, action, perceptionPeriod).Last().state;
                if (collisionDetector.IsCollision(state))
                {
                    return true;
                }
            }

            return false;
        }

        private Queue<IAction>? createNewPlan(IState state)
        {
            var wayPoints = nextGoals(lookahead: 2);
            var planner = new HybridAStarPlanner(perceptionPeriod, vehicleModel, motionModel, track, actions, wayPoints, collisionDetector);
            planner.ExploredStates.Subscribe(x => ExploredStates.OnNext(x));
            var newPlan = planner.FindOptimalPlanFor(state);

            if (newPlan == null)
            {
                return null;
            }

            var plannedActions = new List<IAction>();
            foreach (var action in newPlan.Trajectory.Select(trajectory => trajectory.Action))
            {
                if (action != null)
                {
                    plannedActions.Add(action);
                }
            }

            return new Queue<IAction>(plannedActions);
        }

        private IReadOnlyList<IGoal> nextGoals(int lookahead)
            => pointsToGo.Take(lookahead).ToList().AsReadOnly();
    }
}
