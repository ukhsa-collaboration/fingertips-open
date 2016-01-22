using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class GroupRoot : RootBase
    {
        [JsonProperty]
        public IList<Grouping> Grouping { get; set; }

        [JsonProperty]
        public IList<CoreDataSet> Data { get; set; }

        public GroupRoot()
        {
            Grouping = new List<Grouping>();
            Data = new List<CoreDataSet>();
        }

        [JsonIgnore]
        public Grouping FirstGrouping
        {
            get { return Grouping.FirstOrDefault(); }
        }

        public void Add(Grouping grouping)
        {
            Grouping.Add(grouping);
            IndicatorId = grouping.IndicatorId;
            AreaTypeId = grouping.AreaTypeId;
        }

        public Grouping GetSubnationalGrouping()
        {
            return
                (from g in Grouping where g.ComparatorId == ComparatorIds.Subnational select g).
                    FirstOrDefault();
        }

        public Grouping GetNationalGrouping()
        {
            return
                (from g in Grouping where g.ComparatorId == ComparatorIds.England select g).
                    FirstOrDefault();
        }
    }
}
