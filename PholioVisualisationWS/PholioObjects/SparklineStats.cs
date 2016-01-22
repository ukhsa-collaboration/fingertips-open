
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class SparklineStats
    {
        public SparklineStats(IList<string> xLabels)
        {
            ComparatorValues = new Dictionary<int, IList<string>>();
            XLabels = xLabels;
        }

        [JsonProperty]
        public IList<string> XLabels { get; private set; }

        [JsonProperty]
        public Limits Limits { get; set; }

        [JsonProperty(PropertyName = "IID")]
        public int IndicatorId { get; set; }

        /// <summary>
        /// Key is comparator ID.
        /// </summary>
        [JsonProperty]
        public IDictionary<int, IList<string>> ComparatorValues { get; private set; }

    }
}
