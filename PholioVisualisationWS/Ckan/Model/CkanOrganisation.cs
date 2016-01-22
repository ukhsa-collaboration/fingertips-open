using Newtonsoft.Json;

namespace Ckan.Model
{
    public class CkanOrganisation : BaseCkanObject
    {
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }
}