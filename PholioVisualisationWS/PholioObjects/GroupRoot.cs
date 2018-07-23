using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.PholioObjects
{
    public class GroupRoot : RootBase
    {
        [JsonProperty]
        public IList<Grouping> Grouping { get; set; }

        [JsonProperty]
        public IList<CoreDataSet> Data { get; set; }

        [JsonProperty]
        public int Sequence { get; set; }

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

        [JsonProperty]
        public IndicatorDateChange DateChanges { get; set; }

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
