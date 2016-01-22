using Newtonsoft.Json;

namespace Ckan.Responses
{
    public class BaseCkanResponse
    {
        [JsonProperty(PropertyName = "help")]
        public string Help { get; set; }

        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; } 
    }
}