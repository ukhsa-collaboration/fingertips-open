using System.Collections;
using System.Collections.Generic;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class MostRecentDataForAllCategories : MostRecentData
    {
        public int SexId { get; set; }
        public int AgeId { get; set; }
        public IList<CategoryType> CategoryTypes { get; set; }
    }
}