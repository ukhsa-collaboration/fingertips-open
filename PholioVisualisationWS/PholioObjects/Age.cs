using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class Age
    {
        [JsonProperty]
        public int Id { get; set; }

        [JsonProperty]
        public string Name { get; set; }
    }
}