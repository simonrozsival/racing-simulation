using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Racing.Model;
using Racing.Model.Planning;

namespace Racing.IO.Model
{
    internal sealed class SerializablePlan : IPlan
    {
        public TimeSpan TimeToGoal { get; set; }

        [JsonProperty("trajectory")]
        public IReadOnlyList<SerializableTrajectory> SerializableTrajectory { get; set; }

        [JsonIgnore]
        public IReadOnlyList<IActionTrajectory> Trajectory => SerializableTrajectory;

        public IPlan ToDetailedPlan(IWorldDefinition world)
        {
            throw new NotImplementedException();
        }
    }
}
