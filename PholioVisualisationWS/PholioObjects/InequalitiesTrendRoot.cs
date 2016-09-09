using Newtonsoft.Json;
using System.Collections.Generic;

namespace PholioVisualisation.PholioObjects
{
    public class InequalitiesTrendRoot
    {
        [JsonProperty(PropertyName = "Data")]
        public Dictionary<string, IList<InequalitiesTrendPoint>> DataPoints { get; set; }
        [JsonProperty(PropertyName = "Periods")]
        public string Periods { get; set; }
        [JsonProperty(PropertyName = "Labels")]
        public string Labels { get; set; }
        [JsonProperty(PropertyName = "LabelIds")]
        public string LabelIds { get; set; }
    }
}
