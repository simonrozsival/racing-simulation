using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Racing.Model.Simulation;
using Racing.Model.Vehicle;
using System.Collections.Generic;

namespace Racing.IO.Model
{
    internal class SerialzableSimulationResult
    {
        public double SimulationTime { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Result Result { get; set; }

        public IEnumerable<ISerializableEvent> Log { get; set; } = new List<ISerializableEvent>();

        public SerializableTrack? Track { get; set; }

        public IVehicleModel? VehicleModel { get; set; }
    }
}
