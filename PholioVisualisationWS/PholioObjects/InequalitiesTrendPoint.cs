using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class InequalitiesTrendPoint
    {
        [JsonProperty(PropertyName = ("Value"))]
        public double Value { get; set; }
        [JsonProperty(PropertyName = ("ValueF"))]
        public string ValueF { get; set; }
    }
}
