using System;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class GroupRootSummary : IComparable<GroupRootSummary>
    {
        public string IndicatorName { get; set; }
        [JsonProperty(PropertyName = "IID")]
        public int  IndicatorId { get; set; }
        public int SexId { get; set; }
        public int AgeId { get; set; }
        public int GroupId { get; set; }
        public bool StateSex { get; set; }
        [JsonProperty(PropertyName = "Unit")]
        public Unit IndicatorUnit { get; set; }

        public int CompareTo(GroupRootSummary other)
        {
          var compare= String.Compare(IndicatorName, other.IndicatorName,
                 StringComparison.Ordinal);

            if (compare == 0) 
            {
                compare = GetSequenceId(this.SexId).CompareTo(GetSequenceId(other.SexId));
            }
            return compare;
        }

        private static int GetSequenceId(int sexId)
        {
            if (sexId == SexIds.Persons) return 1;
            return sexId == SexIds.Male ? 2 : 3;
        }
    }
}