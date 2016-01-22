using Newtonsoft.Json;

namespace Ckan.Model
{
    public class BaseCkanObject
    {
        /// <summary>
        /// Name must always be defined
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Only serialise if defined.
        /// </summary>
        public bool ShouldSerializeId()
        {
            return Id != null;
        }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Only serialise if defined.
        /// </summary>
        public bool ShouldSerializeTitle()
        {
            return Title != null;
        }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Do not serialise internal property.
        /// </summary>
        public bool ShouldSerializeType()
        {
            return false;
        }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        /// <summary>
        /// Do not serialise internal property.
        /// </summary>
        public bool ShouldSerializeState()
        {
            return false;
        }
    }
}