using Newtonsoft.Json;
using Racing.IO.Model;
using Racing.Model.Planning;
using System.IO;
using System.Linq;

namespace Racing.IO
{
    public static class Plans
    {
        public static void Store(IPlan plan, string fileName)
        {
            var serializablePlan = new SerializablePlan
            {
                TimeToGoal = plan.TimeToGoal,
                SerializableTrajectory = plan.Trajectory.Select(segment =>
                    new SerializableTrajectory
                    {
                        SerializableAction = segment.Action != null ? new SerializableAction { Steering = segment.Action.Steering, Throttle = segment.Action.Throttle } : null,
                        State = segment.State,
                        TargetWayPoint = segment.TargetWayPoint,
                        Time = segment.Time
                    }).ToList().AsReadOnly()
            };

            var json = JsonConvert.SerializeObject(serializablePlan, CustomJsonSerializationSettings.Default);
            File.WriteAllText(fileName, json);
        }

        public static IPlan Load(string fileName)
        {
            var json = File.ReadAllText(fileName);
            return JsonConvert.DeserializeObject<SerializablePlan>(json, CustomJsonSerializationSettings.Default);
        }
    }
}
