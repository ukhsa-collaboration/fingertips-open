using Newtonsoft.Json;
using System;

namespace Fpm.ProfileData.Entities.DataAudit
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
    }

}
