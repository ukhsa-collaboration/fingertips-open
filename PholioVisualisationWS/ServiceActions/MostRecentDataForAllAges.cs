using System.Collections.Generic;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class MostRecentDataForAllAges : MostRecentData
    {
        public int SexId { get; set; }
        public IList<Age> Ages { get; set; }
    }
}