
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PholioVisualisation.PholioObjects;

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

        /// <summary>
        /// i.e. is not search results, or other kind of ad hoc profile.
        /// </summary>
        [JsonIgnore]
        public bool IsDefinedProfile
        {
            get { return Id != ProfileIds.Search; }
        }


    }
}
