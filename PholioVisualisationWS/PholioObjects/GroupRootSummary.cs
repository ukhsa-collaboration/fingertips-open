using System;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class GroupRootSummary : IComparable<GroupRootSummary>
    {
        public string IndicatorName { get; set; }

        [JsonProperty(PropertyName = "IID")]
        public int IndicatorId { get; set; }

        public Sex Sex { get; set; }
        public Age Age { get; set; }
        public int GroupId { get; set; }
        public bool StateSex { get; set; }
        public bool StateAge { get; set; }

        [JsonProperty(PropertyName = "Unit")]
        public Unit IndicatorUnit { get; set; }

        public int CompareTo(GroupRootSummary other)
        {
            var compare = String.Compare(IndicatorName, other.IndicatorName,
                   StringComparison.Ordinal);

            if (compare == 0)
            {
                compare = GetSequenceId(this.Sex.Id).CompareTo(GetSequenceId(other.Sex.Id));
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