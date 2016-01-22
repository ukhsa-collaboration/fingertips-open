
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class Unit
    {
        [JsonProperty]
        public int Id { get; set; }

        [JsonProperty]
        public double Value { get; set; }

        [JsonProperty]
        public string Label { get; set; }

        [JsonProperty(PropertyName = "ShowLeft")]
        public bool ShowLabelOnLeftOfValue { get; set; }

        /// <summary>
        /// Whether or not property should be serialised to JSON.
        /// </summary>
        public bool ShouldSerializeShowLabelOnLeftOfValue()
        {
            return ShowLabelOnLeftOfValue;
        }
    }
}
