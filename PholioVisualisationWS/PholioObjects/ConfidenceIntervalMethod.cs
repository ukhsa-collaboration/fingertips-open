
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class ConfidenceIntervalMethod
    {
        // NOTE: Usage field is not for display, it is for internal info

        [JsonProperty]
        public int Id { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public string Description { get; set; }
    }
}
