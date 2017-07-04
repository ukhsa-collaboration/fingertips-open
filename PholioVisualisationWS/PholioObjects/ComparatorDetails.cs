
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class ComparatorDetails
    {
        [JsonProperty]
        public IArea Area { get; set; }

        [JsonProperty(PropertyName = "AreaTypeId")]
        public int ChildAreaTypeId { get; set; }

        [JsonProperty]
        public int ComparatorId { get; set; }

        public override bool Equals(object obj)
        {
            ComparatorDetails other = obj as ComparatorDetails;

            if (other == null)
            {
                return false;
            }

            return other.Area.Code == Area.Code &&
                other.ChildAreaTypeId == ChildAreaTypeId &&
                other.ComparatorId == ComparatorId;
        }

        public override int GetHashCode()
        {
            return (Area.Code + ChildAreaTypeId + ComparatorId).GetHashCode();
        }
    }
}
