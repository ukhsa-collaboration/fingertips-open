using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class Age : INamedEntity
    {
        [JsonProperty]
        public int Id { get; set; }

        [JsonProperty]
        public string Name { get; set; }
    }
}