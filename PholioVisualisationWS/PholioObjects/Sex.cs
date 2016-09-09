using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class Sex : INamedEntity
    {
        [JsonProperty]
        public int Id { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonIgnore]
        public int Sequence { get; set; } 
    }
}