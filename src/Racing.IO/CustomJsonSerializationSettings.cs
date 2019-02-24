using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Racing.IO
{
    internal static class CustomJsonSerializationSettings
    {
        public static readonly JsonSerializerSettings Default = 
            new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
    }
}
