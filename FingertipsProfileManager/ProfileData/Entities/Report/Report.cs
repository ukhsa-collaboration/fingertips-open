using Newtonsoft.Json;

namespace Fpm.ProfileData.Entities.Report
{
    public class Report
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "file")]
        public string File { get; set; }
        [JsonProperty(PropertyName = "parameters")]
        public string Parameters { get; set; }
        [JsonProperty(PropertyName = "notes")]
        public string Notes { get; set; }
        [JsonProperty(PropertyName = "isLive")]
        public bool IsLive { get; set; }
        [JsonProperty(PropertyName = "areaTypeIds")]
        public string AreaTypeIds { get; set; }
    }
}
