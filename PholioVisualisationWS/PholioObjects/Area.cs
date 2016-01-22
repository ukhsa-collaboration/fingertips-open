using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class Area : IArea
    {
        [JsonProperty]
        public string Code { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Short")]
        public string ShortName { get; set; }

        public bool ShouldSerializeShortName()
        {
            return ShortName != null;
        }

        [JsonProperty]
        public int AreaTypeId { get; set; }

        [JsonIgnore]
        public bool IsCurrent { get; set; }

        [JsonIgnore]
        public int? Sequence { get; set; }

        /// <summary>
        /// Whether or not the area is a CCG.
        /// </summary>
        [JsonIgnore]
        public bool IsCcg
        {
            get { return AreaTypeId == AreaTypeIds.Ccg; }
        }

        /// <summary>
        /// Whether or not the area is a County.
        /// </summary>
        [JsonIgnore]
        public bool IsCounty
        {
            get { return AreaTypeId == AreaTypeIds.County; }
        }

        /// <summary>
        /// Whether or not the area is a Unitary Authority.
        /// </summary>
        [JsonIgnore]
        public bool IsUa
        {
            get { return AreaTypeId == AreaTypeIds.UnitaryAuthority; }
        }

        /// <summary>
        /// Whether or not the area is a deprivation decile.
        /// </summary>
        [JsonIgnore]
        public bool IsGpDeprivationDecile
        {
            get { return false; }
        }

        /// <summary>
        /// Whether or not the area is a practice shape.
        /// </summary>
        [JsonIgnore]
        public bool IsShape
        {
            get { return AreaTypeId == AreaTypeIds.Shape; }
        }

        /// <summary>
        /// Whether or not the area is a GP practice.
        /// </summary>
        [JsonIgnore]
        public bool IsGpPractice
        {
            get { return AreaTypeId == AreaTypeIds.GpPractice; }
        }

        /// <summary>
        /// Whether or not the area is a PCT.
        /// </summary>
        [JsonIgnore]
        public bool IsPct
        {
            get { return AreaTypeId == AreaTypeIds.Pct; }
        }

        /// <summary>
        /// Whether or not the area is a country, e.g. England.
        /// </summary>
        [JsonIgnore]
        public bool IsCountry
        {
            get { return AreaTypeId == AreaTypeIds.Country; }
        }

        /// <summary>
        /// Whether or not the area is a PHE centre.
        /// </summary>
        [JsonIgnore]
        public bool IsPheCentre
        {
            get { return AreaTypeId == AreaTypeIds.PheCentreObsolete; }
        }

        /// <summary>
        /// Whether or not the area is a county or UA deprivation decile
        /// </summary>
        [JsonIgnore]
        public bool IsCountyAndUnitaryAuthorityDeprivationDecile
        {
            get { return false; }
        }

        /// <summary>
        /// Whether or not the area code is nearest neighbour.
        /// </summary>
        public static bool IsNearestNeighbour(string areaCode)
        {
            if (string.IsNullOrWhiteSpace(areaCode))
            {
                return false;
            }

            return areaCode.ToLower().StartsWith("nn-1");
        }
    }
}
