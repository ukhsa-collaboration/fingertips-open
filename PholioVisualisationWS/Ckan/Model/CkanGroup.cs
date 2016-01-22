
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ckan.Model
{
    public class CkanGroup : BaseCkanObject
    {
        [JsonProperty(PropertyName = "created")]
        public DateTime Created { get; set; }

        /// <summary>
        /// Do not serialise internal property.
        /// </summary>
        public bool ShouldSerializeCreated()
        {
            return false;
        }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Only serialise if defined.
        /// </summary>
        public bool ShouldSerializeDescription()
        {
            return Description != null;
        }

        [JsonProperty(PropertyName = "revision_id")]
        public string RevisionId { get; set; }

        /// <summary>
        /// Only serialise if defined.
        /// </summary>
        public bool ShouldSerializeRevisionId()
        {
            return RevisionId != null;
        }

        /// <summary>
        /// The URL to an image to be displayed on the group’s page 
        /// </summary>
        [JsonProperty(PropertyName = "image_url")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Only serialise if defined.
        /// </summary>
        public bool ShouldSerializeImageUrl()
        {
            return ImageUrl != null;
        }

        [JsonProperty(PropertyName = "image_display_url")]
        public string ImageDisplayUrl { get; set; }

        /// <summary>
        /// Only serialise if defined.
        /// </summary>
        public bool ShouldSerializeImageDisplayUrl()
        {
            return ImageDisplayUrl != null;
        }

        [JsonProperty(PropertyName = "approval_status")]
        public string ApprovalStatus { get; set; }

        /// <summary>
        /// Do not serialise internal property.
        /// </summary>
        public bool ShouldSerializeApprovalStatus()
        {
            return false;
        }

        [JsonProperty(PropertyName = "package_count")]
        public int PackageCount { get; set; }

        /// <summary>
        /// Do not serialise derived property.
        /// </summary>
        public bool ShouldSerializePackageCount()
        {
            return false;
        }

        [JsonProperty(PropertyName = "num_followers")]
        public int NumberOfFollowers { get; set; }

        /// <summary>
        /// Do not serialise derived property.
        /// </summary>
        public bool ShouldSerializeNumberOfFollowers()
        {
            return false;
        }

        [JsonProperty(PropertyName = "is_organisation")]
        public bool IsOrganisation { get; set; }

        /// <summary>
        /// Do not serialise internal property.
        /// </summary>
        public bool ShouldSerializeIsOrganisation()
        {
            return false;
        }

        [JsonProperty(PropertyName = "packages")]
        public List<CkanPackage> Packages { get; set; }

        public bool ShouldSerializePackages()
        {
            return Packages != null;
        }

        [JsonProperty(PropertyName = "users")]
        public List<CkanUser> Users { get; set; }

        /// <summary>
        /// We should never need to modify the user list but do need to include
        /// it in our response to avoid unintentionally clearing it.
        /// </summary>
        public bool ShouldSerializeUsers()
        {
            return Users != null && Users.Any();
        }

        public static string GetNewName(string profileUrlKey)
        {
            return "phe-" + profileUrlKey;
        }

        /// <summary>
        /// Get the most minimally defined group for posting to CKAN. Use when 
        /// want to associate a package with a group. If too many properties are sent
        /// then CKAN complains about "__junk".
        /// </summary>
        public CkanGroup GetMinimalGroupForSendingToCkan()
        {
            return new CkanGroup
            {
                Id = Id,
                Name = Name
            };
        }
    }
}