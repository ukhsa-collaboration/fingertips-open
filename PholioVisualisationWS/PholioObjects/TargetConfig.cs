
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    /// <summary>
    /// DTO for NHibernate
    /// </summary>
    public class TargetConfig
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonProperty]
        public double? LowerLimit { get; set; }

        [JsonProperty]
        public double? UpperLimit { get; set; }

        [JsonProperty(PropertyName = "BespokeKey")]
        public string BespokeTargetKey { get; set; }

        [JsonProperty]
        public int PolarityId { get; set; }

        [JsonIgnore]
        public string Description { get; set; }
    }
}
