using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class Limits
    {
        [JsonProperty]
        public double Min { get; set; }

        [JsonProperty]
        public double Max { get; set; }

    }
}
