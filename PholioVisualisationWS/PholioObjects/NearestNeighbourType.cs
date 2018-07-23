using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class NearestNeighbourType
    {
        [JsonProperty]
        public int NeighbourTypeId { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public string Description { get; set; }

        [JsonProperty]
        public string LinkText { get; set; }

        [JsonProperty]
        public string SelectedText { get; set; }

        [JsonProperty]
        public string ExtraLink { get; set; }

        /// <summary>
        /// Whether or not Name should be serialised to JSON.
        /// </summary>
        public bool ShouldSerializeName()
        {
            return Name != null;
        }

        /// <summary>
        /// Whether or not Description should be serialised to JSON.
        /// </summary>
        public bool ShouldSerializeDescription()
        {
            return Description != null;
        }

        /// <summary>
        /// Whether or not LinkText should be serialised to JSON.
        /// </summary>
        public bool ShouldSerializeLinkText()
        {
            return LinkText != null;
        }

        /// <summary>
        /// Whether or not SelectedText should be serialised to JSON.
        /// </summary>
        public bool ShouldSerializeSelectedText()
        {
            return SelectedText != null;
        }

        /// <summary>
        /// Whether or not ExtraLink should be serialised to JSON.
        /// </summary>
        public bool ShouldSerializeExtraLink()
        {
            return ExtraLink != null;
        }

        public void ClearSystemContent()
        {
            LinkText = null;
            SelectedText = null;
        }
    }
}
