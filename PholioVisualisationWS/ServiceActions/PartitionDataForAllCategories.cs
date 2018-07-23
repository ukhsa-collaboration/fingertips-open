using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class PartitionDataForAllCategories : PartitionData
    {
        public int SexId { get; set; }
        public int AgeId { get; set; }
        public IList<CategoryType> CategoryTypes { get; set; }

        [JsonProperty]
        public IList<CoreDataSet> BenchmarkDataSpecialCases { get; set; }

        public bool ShouldSerializeBenchmarkDataSpecialCases()
        {
            return BenchmarkDataSpecialCases != null && BenchmarkDataSpecialCases.Any();
        }
    }
}