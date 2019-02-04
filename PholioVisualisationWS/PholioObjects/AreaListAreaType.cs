using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class AreaListAreaType : IAreaType
    {
        [JsonProperty]
        public int Id { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Short")]
        public string ShortName { get; set; }

        public AreaListAreaType()
        {
            Id = AreaTypeIds.AreaList;
            Name = "Your area list";
            ShortName = Name;
        }

        public static bool IsAreaListAreaType(int areaTypeId)
        {
            return areaTypeId == AreaTypeIds.AreaList;
        }
    }
}
