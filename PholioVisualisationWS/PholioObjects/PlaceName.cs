using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class PlaceName
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PlaceType { get; set; }
        public string Postcode { get; set; }
        public int Easting { get; set; }
        public int Northing { get; set; }

        public PostcodeParentAreas PostcodeParentAreas { get; set; }

        [JsonIgnore]
        public int PlaceTypeWeighting
        {
            get
            {
                var lowerPlaceType = PlaceType.ToLower();

                if (lowerPlaceType == "c")
                {
                    // City
                    return 4;
                }

                if (lowerPlaceType == "t")
                {
                    // Town
                    return 2;
                }

                return 1;
            }
        }
    }
}