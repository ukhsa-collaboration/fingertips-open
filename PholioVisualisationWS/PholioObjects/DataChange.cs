using Newtonsoft.Json;
using System;

namespace PholioVisualisation.PholioObjects
{
    public class DataChange
    {
        [JsonProperty]
        public string LastUploadedBy { get; set; }

        [JsonProperty]
        public DateTime LastUploadedAt { get; set; }

        [JsonProperty]
        public string LastDeletedBy { get; set; }

        [JsonProperty]
        public DateTime LastDeletedAt { get; set; }

        [JsonIgnore]
        public bool ShouldSerializeUserNames { get; set; }

        public bool ShouldSerializeLastUploadedBy()
        {
            return ShouldSerializeUserNames;
        }

        public bool ShouldSerializeLastDeletedBy()
        {
            return ShouldSerializeUserNames;
        }
    }
}
