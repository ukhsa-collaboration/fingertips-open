using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class ComparatorMethod
    {
        [JsonProperty]
        public int Id { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public string ShortName { get; set; }

        [JsonProperty]
        public string Description { get; set; }

        [JsonProperty]
        public string Reference { get; set; }

        [JsonIgnore]
        public bool IsCurrent { get; set; }
    }
}