using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public interface IInequalitiesTrendOptions
    {
        [JsonProperty(PropertyName = "Id")]
        int Id { get; set; }
        [JsonProperty(PropertyName = "Name")]
        string Name { get; set; }
        [JsonProperty(PropertyName = "AreaCode")]
        string AreaCode { get; set; }
    }
}
