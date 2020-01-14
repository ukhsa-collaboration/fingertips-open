using System.Collections.Generic;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class PartitionDataForAllAges : PartitionData
    {
        public int SexId { get; set; }
        public IList<Age> Ages { get; set; }
        public int ChartAverageLineAgeId { get; set; }
    }
}