using Newtonsoft.Json;
using Racing.IO.Model;
using Racing.Model;
using Racing.Model.Simulation;
using Racing.Model.Vehicle;
using System.IO;
using System.Linq;

namespace Racing.IO
{
    public static class Simulation
    {
        public static void StoreResult(ITrack track, IVehicleModel vehicle, ISummary summary, string imageFilePath, string fileName)
        {
            var serializableSummary = new SerialzableSimulationResult
            {
                SimulationTime = summary.SimulationTime.TotalSeconds,
                Result = summary.Result,
                Log = summary.Log.Select(SerializableEventFactory.From),
                Track = SerializableTrack.From(track.TileSize.Meters, track.Circuit, track.OccupancyGrid),
                VehicleModel = vehicle,
                TrackImageFilePath = imageFilePath
            };

            var json = JsonConvert.SerializeObject(serializableSummary, CustomJsonSerializationSettings.Default);
            File.WriteAllText(fileName, json);
        }
    }
}
