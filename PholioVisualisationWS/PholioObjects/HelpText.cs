using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class HelpText
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonProperty]
        public string Key { get; set; }

        [JsonProperty]
        public string Text { get; set; }
    }
}
