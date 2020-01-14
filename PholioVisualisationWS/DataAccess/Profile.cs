
using Newtonsoft.Json;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;

namespace PholioVisualisation.DataAccess
{
    public class Profile
    {
        [JsonProperty]
        public int Id { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Key")]
        public string UrlKey { get; set; }

        [JsonIgnore]
        public bool IsNational { get; set; }

        [JsonProperty]
        public List<int> GroupIds { get; set; }

        [JsonIgnore]
        public bool HasTrendMarkers { get; set; }

        public Profile(IEnumerable<int> groupIds)
        {
            GroupIds = new List<int>(groupIds);
        }

        [JsonProperty]
        public IList<GroupingMetadata> GroupMetadata { get; set; }

        [JsonIgnore]
        public bool AreIndicatorNamesDisplayedWithNumbers { get; set; }
    }
}
