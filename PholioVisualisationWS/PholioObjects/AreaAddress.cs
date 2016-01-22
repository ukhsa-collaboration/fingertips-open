
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class AreaAddress
    {
        [JsonProperty]
        public string Code { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Short")]
        public string ShortName { get; set; }

        [JsonProperty]
        public int AreaTypeId { get; set; }

        [JsonProperty(PropertyName = "A1")]
        public string Address1 { get; set; }

        [JsonProperty(PropertyName = "A2")]
        public string Address2 { get; set; }

        [JsonProperty(PropertyName = "A3")]
        public string Address3 { get; set; }

        [JsonProperty(PropertyName = "A4")]
        public string Address4 { get; set; }

        [JsonProperty(PropertyName = "Postcode")]
        public string Postcode { get; set; }

        [JsonProperty(PropertyName = "IsCurrent")]
        public bool IsCurrent { get; set; }

        [JsonIgnore]
        public EastingNorthing EastingNorthing { get; set; }

        [JsonProperty(PropertyName = "Pos")]
        public LatitudeLongitude LatitudeLongitude { get; set; }

        public void CleanAddress()
        {
            Address1 = Trim(Address1);
            Address2 = Trim(Address2);
            Address3 = Trim(Address3);
            Address4 = Trim(Address4);
            Postcode = Trim(Postcode);
        }

        private static string Trim(string s)
        {
            return s == null ?
                null :
                s.Trim().Trim(',');
        }
    }
}
