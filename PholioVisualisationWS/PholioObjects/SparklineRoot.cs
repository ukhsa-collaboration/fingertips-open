
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class SparklineRoot
    {
        [JsonProperty]
        public SparklineStats Stats { get; set; }

        /// <summary>
        /// Key is area code.
        /// </summary>
        public IDictionary<string, SparklineArea> AreaData { get; set; }

        public SparklineRoot()
        {
            AreaData = new Dictionary<string, SparklineArea>();
        }
    }
}
