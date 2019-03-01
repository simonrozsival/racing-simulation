using Newtonsoft.Json;
using Racing.Model;
using Racing.Model.Planning;
using Racing.Model.Vehicle;
using System;

namespace Racing.IO.Model
{
    internal sealed class SerializableTrajectory : IActionTrajectory
    {
        public TimeSpan Time { get; set; }

        public VehicleState State { get; set; }

        [JsonIgnore]
        public IAction? Action => SerializableAction;

        [JsonProperty("action")]
        public SerializableAction? SerializableAction { get; set; }

        public int TargetWayPoint { get; set; }
    }
}
