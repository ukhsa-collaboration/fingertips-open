using Newtonsoft.Json;
using System;

namespace Ckan.Model
{
    public class CkanUser
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "display_name")]
        public string DisplayName { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "fullname")]
        public string Fullname { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "number_of_edits")]
        public int NumberOfEdits { get; set; }

        [JsonProperty(PropertyName = "number_administered_packages")]
        public int NumberAdministeredPackages { get; set; }

        [JsonProperty(PropertyName = "activity_streams_email_notifications")]
        public bool ActivityStreamsEmailNotifications { get; set; }

        [JsonProperty(PropertyName = "sysadmin")]
        public bool SysAdmin { get; set; }

        [JsonProperty(PropertyName = "created")]
        public DateTime Created { get; set; }

        [JsonProperty(PropertyName = "openid")]
        public string OpenId { get; set; }

        [JsonProperty(PropertyName = "capacity")]
        public string Capacity { get; set; }

        [JsonProperty(PropertyName = "about")]
        public string About { get; set; }

        [JsonProperty(PropertyName = "email_hash")]
        public string EmailHash { get; set; }
    }
}