
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class SparklineTimePoint
    {
        /// <summary>
        /// Or signifance
        /// </summary>
        [JsonProperty]
        public bool AreDifferent { get; set; }

        /// <summary>
        /// Key is type of value, e.g. 80/20
        /// </summary>
        [JsonProperty]
        public IDictionary<string, ValueWithCIsData> Data { get; set; }

        public SparklineTimePoint()
        {
            Data = new Dictionary<string, ValueWithCIsData>();
        }
    }
}
