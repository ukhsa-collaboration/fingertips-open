using System.Collections;
using System.Collections.Generic;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class PartitionDataForAllCategories : PartitionData
    {
        public int SexId { get; set; }
        public int AgeId { get; set; }
        public IList<CategoryType> CategoryTypes { get; set; }
    }
}