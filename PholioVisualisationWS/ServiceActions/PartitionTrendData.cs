using Newtonsoft.Json;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;

namespace PholioVisualisation.ServiceActions
{
    public class PartitionTrendData
    {
        [JsonProperty]
        public IList<INamedEntity> Labels { get; set; }

        [JsonProperty]
        public Dictionary<int, IList<CoreDataSet>> TrendData { get; set; }

        [JsonProperty]
        public IList<string> Periods { get; set; }

        [JsonProperty]
        public Limits Limits { get; set; }

        [JsonProperty]
        public IList<CoreDataSet> AreaAverage { get; set; }

    }
}
