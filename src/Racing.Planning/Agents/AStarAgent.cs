using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Racing.Planning.Algorithms.Domain;
using Racing.Model;

namespace Racing.Planning.Agents
{
    public class AStarAgent : IAgent
    {
        private readonly TimeSpan perceptionPeriod;
        private readonly IWorldDefinition world;
        private readonly IReadOnlyList<IGoal> wayPoints;

        private Queue<IAction>? plan = null;
        private int previousTargetWayPoint;

        public ISubject<IState> ExploredStates { get; } = new Subject<IState>();

        public AStarAgent(
            IWorldDefinition world,
            TimeSpan perceptionPeriod)
        {
            this.world = world;
            this.perceptionPeriod = perceptionPeriod;

            wayPoints = world.Track.Circuit.WayPoints.ToList().AsReadOnly();
        }

        public IAction ReactTo(IState state, int wayPoint)
        {
            if (previousTargetWayPoint != wayPoint)
            {
                if (wayPoint >= wayPoints.Count)
                {
                    // no more way points to reach
                    return world.Actions.Brake;
                }

                // the agent must have passed a way point since the last time
                plan = createNewPlan(state, wayPoint);
            }

            previousTargetWayPoint = wayPoint;

            if (plan == null || plan.Count == 0)
            {
                plan = createNewPlan(state, wayPoint);
            }

            if (plan == null || plan.Count == 0)
            {
                return world.Actions.Brake;
            }

            var nextAction = plan.Dequeue();

            if (nextAction != null && couldLeadToCrash(state, nextAction))
            {
                plan = createNewPlan(state, wayPoint);
                if (plan != null && plan.Count != 0)
                {
                    nextAction = plan.Dequeue();
                }
            }

            // todo: is too off the planned trajectory?

            return nextAction ?? world.Actions.Brake;
        }

        private bool couldLeadToCrash(IState state, IAction action, int lookahead = 3)
        {
            for (int i = 0; i < lookahead; i++)
            {
                state = world.MotionModel.CalculateNextState(state, action, perceptionPeriod).Last().state;
                if (world.CollisionDetector.IsCollision(state))
                {
                    return true;
                }
            }

            return false;
        }

        private Queue<IAction>? createNewPlan(IState state, int wayPoint)
        {
            var wayPoints = nextGoals(wayPoint, lookahead: 2);
            var planner = new HybridAStarPlanner(perceptionPeriod, world, wayPoints, greedy: true);
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

        private IReadOnlyList<IGoal> nextGoals(int wayPoint, int lookahead)
            => wayPoints.Skip(wayPoint).Take(lookahead).ToList().AsReadOnly();
    }
}
