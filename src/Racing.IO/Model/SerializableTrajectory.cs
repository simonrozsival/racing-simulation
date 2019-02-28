using Newtonsoft.Json;
using Racing.Model;
using Racing.Model.Planning;
using System;

namespace Racing.IO.Model
{
    internal sealed class SerializableTrajectory : IActionTrajectory
    {
        public TimeSpan Time { get; set; }

        [JsonIgnore]
        public IState State => SerializableState;

        [JsonProperty("state")]
        public SerializableState SerializableState { get; set; }

        [JsonIgnore]
        public IAction? Action => SerializableAction;

        [JsonProperty("action")]
        public SerializableAction? SerializableAction { get; set; }

        public int TargetWayPoint { get; set; }
    }
}
