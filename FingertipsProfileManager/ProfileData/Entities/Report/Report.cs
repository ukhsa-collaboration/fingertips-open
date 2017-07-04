using Newtonsoft.Json;

namespace Fpm.ProfileData.Entities.Report
{
    public class Report
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "parameters")]
        public string Parameters { get; set; }
    }
}
