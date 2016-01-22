using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ckan.Model
{
    public class CkanResource
    {
        [JsonProperty(PropertyName = "package_id")]
        public string PackageId { get; set; }

        /// <summary>
        /// URL for resources that are not hosted on CKAN itself.
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "resource_group_id")]
        public string ResourceGroupId { get; set; }

        [JsonProperty(PropertyName = "revision_id")]
        public string RevisionId { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "format")]
        public string Format { get; set; }

        [JsonProperty(PropertyName = "hash")]
        public string Hash { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "resource_type")]
        public string ResourceType { get; set; }

        [JsonProperty(PropertyName = "size")]
        public int? Size { get; set; }

        [JsonProperty(PropertyName = "created")]
        public DateTime Created { get; set; }

        [JsonIgnore]
        public  CkanResourceFile File { get; set; }
    }
}
