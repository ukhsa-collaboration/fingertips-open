using Newtonsoft.Json;
using System.Collections.Generic;

namespace PholioVisualisation.PholioObjects
{
    public class AreaType : IAreaType
    {
        private static IList<int> _ccgAreaTypeIds = new List<int>
        {
            AreaTypeIds.CcgsPostApr2017,
            AreaTypeIds.CcgsPostApr2018,
            AreaTypeIds.CcgsPostApr2019
        };

        private static IList<int> _pheCentreAreaTypeIds = new List<int>
        {
            AreaTypeIds.PheCentresFrom2013To2015,
            AreaTypeIds.PheCentresFrom2015
        };


        public int Id { get; set; }

        [JsonProperty] public string Name { get; set; }

        [JsonProperty(PropertyName = "Short")] public string ShortName { get; set; }

        [JsonIgnore] public bool IsCurrent { get; set; }

        [JsonIgnore] public bool IsSupported { get; set; }

        /// <summary>
        /// Is searchable by placename and postcode
        /// </summary>
        [JsonProperty] public bool IsSearchable { get; set; }

        [JsonProperty] public bool CanBeDisplayedOnMap { get; set; }

        [JsonProperty] public IList<IAreaType> ParentAreaTypes { get; set; }

        /// <summary>
        ///     Whether or not ParentAreaTypes should be serialised to JSON.
        /// </summary>
        public bool ShouldSerializeParentAreaTypes()
        {
            return ParentAreaTypes != null;
        }

        public static bool IsCcgAreaTypeId(int areaTypeId)
        {
            return _ccgAreaTypeIds.Contains(areaTypeId);
        }

        public static bool IsPheCentreAreaTypeId(int areaTypeId)
        {
            return _pheCentreAreaTypeIds.Contains(areaTypeId);
        }

        public bool IsEngland()
        {
            return Id == AreaTypeIds.Country;
        }
    }
}
