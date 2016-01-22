using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Ckan.Model
{
    public class CkanPackage : BaseCkanObject
    {
        [JsonProperty(PropertyName = "metadata_created")]
        public DateTime MetadataCreated { get; set; }

        /// <summary>
        /// Do not serialize internal property.
        /// </summary>
        public bool ShouldSerializeMetadataCreated()
        {
            return false;
        }

        [JsonProperty(PropertyName = "metadata_modified")]
        public DateTime MetadataModified { get; set; }

        /// <summary>
        /// Do not serialize internal property.
        /// </summary>
        public bool ShouldSerializeMetadataModified()
        {
            return false;
        }

        [JsonProperty(PropertyName = "revision_timestamp")]
        public DateTime RevisionTimestamp { get; set; }

        /// <summary>
        /// Do not serialize internal property.
        /// </summary>
        public bool ShouldSerializeRevisionTimestamp()
        {
            return false;
        }

        [JsonProperty(PropertyName = "author")]
        public string Author { get; set; }

        [JsonProperty(PropertyName = "author_email")]
        public string AuthorEmail { get; set; }

        [JsonProperty(PropertyName = "maintainer")]
        public string Maintainer { get; set; }

        [JsonProperty(PropertyName = "maintainer_email")]
        public string MaintainerEmail { get; set; }

        [JsonProperty(PropertyName = "notes")]
        public string Notes { get; set; }

        /// <summary>
        /// A link to the source of the dataset.
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public string Source { get; set; }

        [JsonProperty(PropertyName = "homepage")]
        public string Homepage { get; set; }

        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        [JsonProperty(PropertyName = "revision_id")]
        public string RevisionId { get; set; }

        /// <summary>
        /// Do not serialize internal property.
        /// </summary>
        public bool ShouldSerializeRevisionId()
        {
            return false;
        }

        [JsonProperty(PropertyName = "creator_user_id")]
        public string CreatorUserId { get; set; }

        [JsonProperty(PropertyName = "license_id")]
        public string LicenseId { get; set; }

        [JsonProperty(PropertyName = "license_title")]
        public string LicenseTitle { get; set; }

        [JsonProperty(PropertyName = "private")]
        public bool Private { get; set; }

        /// <summary>
        /// Do not serialize internal property.
        /// </summary>
        public bool ShouldSerializePrivate()
        {
            return false;
        }

        [JsonProperty(PropertyName = "isopen")]
        public bool IsOpen { get; set; }

        /// <summary>
        /// Do not serialize internal property.
        /// </summary>
        public bool ShouldSerializeIsOpen()
        {
            return false;
        }

        [JsonProperty(PropertyName = "organization")]
        public CkanOrganisation Publisher { get; set; }

        /// <summary>
        /// ID of the dataset’s owning organization
        /// </summary>
        [JsonProperty(PropertyName = "owner_org")]
        public string OwnerOrganization { get; set; }

        [JsonProperty(PropertyName = "groups")]
        public List<CkanGroup> Groups { get; set; }

        [JsonProperty(PropertyName = "frequency")]
        public List<string> Frequency { get; set; }

        /// <summary>
        /// Use Source in PHOLIO metadata
        /// </summary>
        [JsonProperty(PropertyName = "origin")]
        public string Origin { get; set; }

        /// <summary>
        /// Format "yyyy-mm-dd"
        /// </summary>
        [JsonProperty(PropertyName = "coverage_end_date")]
        public string CoverageEndDate { get; set; }

        /// <summary>
        /// Format "yyyy-mm-dd"
        /// </summary>
        [JsonProperty(PropertyName = "coverage_start_date")]
        public string CoverageStartDate { get; set; }

        [JsonProperty(PropertyName = "resources")]
        public List<CkanResource> Resources { get; set; }

        public bool ShouldSerializeResources()
        {
            return Resources != null;
        }

        [JsonProperty(PropertyName = "num_resources")]
        public int NumberOfResources { get; set; }

        /// <summary>
        /// Do not serialize derived property.
        /// </summary>
        public bool ShouldSerializeNumberOfResources()
        {
            return false;
        }

        /// <summary>
        /// Whether or not this instance has been deserialised from
        /// a response from a CKAN repository.
        /// </summary>
        [JsonIgnore]
        public bool IsInstanceFromRepository
        {
            get { return MetadataCreated.Year != 1; }
        }
    }
}