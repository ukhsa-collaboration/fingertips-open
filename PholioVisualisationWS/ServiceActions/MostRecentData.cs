using System.Collections.Generic;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class MostRecentData
    {
        public string AreaCode { get; set; }
        public int IndicatorId { get; set; }
        public IList<CoreDataSet> Data { get; set; } 
    }
}