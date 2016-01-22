
using System.Text;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class ProfilePerIndicator
    {
        [JsonIgnore]
        public int IndicatorId { get; set; }
        [JsonIgnore]
        public int GroupId { get; set; }
        [JsonIgnore]
        public int AreaTypeId { get; set; }
        [JsonIgnore]
        public int ProfileId { get; set; }

        public string ProfileName { get; set; }
        
        [JsonIgnore]
        public string ProfileUrl { get; set; }
        [JsonIgnore]
        public string LiveHostUrl { get; set; }
        [JsonIgnore]
        public string TestHostUrl { get; set; }

        public string Url { get; set; }

    }
}
