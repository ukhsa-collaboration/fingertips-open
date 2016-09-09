
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class GeographicalSearchResult
    {
        [JsonProperty]
        public string PlaceName { get; set; }

        /// <summary>
        /// In UI PolygonAreaName is now used instead of county.
        /// </summary>
        [JsonIgnore]
        public string County { get; set; }

        [JsonProperty]
        public string PolygonAreaCode { get; set; }

        [JsonProperty]
        public string PolygonAreaName { get; set; }

        [JsonProperty]
        public int Easting { get; set; }

        [JsonProperty]
        public int Northing { get; set; }

        /// <summary>
        ///     Whether or not PolygonAreaCode should be serialised to JSON.
        /// </summary>
        public bool ShouldSerializePolygonAreaCode()
        {
            return PolygonAreaCode != null;
        }

        public bool ShouldSerializeEasting()
        {
            return Easting > 0;
        }

        public bool ShouldSerializeNorthing()
        {
            return Northing > 0;
        }

        /// <summary>
        /// Are Easting and Northing both valid?
        /// </summary>
        public bool AreCoordinatesValid()
        {
            return Easting != 0 &&
                Northing != 0;
        }
    }
}
