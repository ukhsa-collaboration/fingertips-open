using System.Collections.Generic;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class LongerLivesAreaDetails
    {
        public IArea Area { get; set; }
        public Decile Decile { get; set; }
        public string Url { get; set; }
        public Dictionary<string, List<AreaRankGrouping>> Ranks { get; set; }
        public Dictionary<string, List<int?>> Significances { get; set; }
        public Dictionary<string, object> Benchmarks { get; set; }
    }
}