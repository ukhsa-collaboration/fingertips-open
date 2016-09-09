using System.Collections.Generic;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class PartitionDataForAllSexes : PartitionData
    {
        public int AgeId { get; set; }
        public IList<Sex> Sexes { get; set; } 
    }
}