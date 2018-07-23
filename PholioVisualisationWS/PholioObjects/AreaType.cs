using Newtonsoft.Json;
using System.Collections.Generic;

namespace PholioVisualisation.PholioObjects
{
    public class AreaType : IAreaType
    {
        public int Id { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Short")]
        public string ShortName { get; set; }

        [JsonIgnore]
        public bool IsCurrent { get; set; }

        [JsonIgnore]
        public bool IsSupported { get; set; }

        [JsonProperty]
        public bool IsSearchable { get; set; }

        [JsonProperty]
        public bool CanBeDisplayedOnMap { get; set; }

        [JsonProperty]
        public IList<IAreaType> ParentAreaTypes { get; set; }

        /// <summary>
        ///     Whether or not ParentAreaTypes should be serialised to JSON.
        /// </summary>
        public bool ShouldSerializeParentAreaTypes()
        {
            return ParentAreaTypes != null;
        }
    }
}
