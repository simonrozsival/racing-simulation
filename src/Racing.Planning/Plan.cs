using Racing.Model;
using Racing.Model.Planning;
using System;
using System.Collections.Generic;

namespace Racing.Planning
{
    internal sealed class Plan : IPlan
    {
        public Plan(TimeSpan timeToGoal, IReadOnlyList<IActionTrajectory> trajectory)
        {
            TimeToGoal = timeToGoal;
            Trajectory = trajectory;
        }

        public TimeSpan TimeToGoal { get; }
        public IReadOnlyList<IActionTrajectory> Trajectory { get; }

        public IPlan ToDetailedPlan(IWorldDefinition world)
        {
            var detailedTrajectory = new List<IActionTrajectory>();

            for (int i = 0; i < Trajectory.Count; i++)
            {
                var trajectory = Trajectory[i];
                var state = trajectory.State;
                var action = trajectory.Action;

                if (action != null)
                {
                    var nextTrajectorySegmentTime = i < Trajectory.Count - 1
                        ? Trajectory[i + 1].Time
                        : TimeToGoal;
                    var timeStep = nextTrajectorySegmentTime - trajectory.Time;

                    var predictions = world.MotionModel.CalculateNextState(state, action, timeStep);
                    var previousState = trajectory.State;
                    foreach (var (time, predictedState) in predictions)
                    {
                        var currentTime = trajectory.Time + time;
                        detailedTrajectory.Add(new ActionTrajectory(currentTime, predictedState, action, trajectory.TargetWayPoint));
                        previousState = predictedState;
                    }
                }
            }

            return new Plan(TimeToGoal, detailedTrajectory);
        }
    }
}
