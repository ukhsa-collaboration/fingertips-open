using Newtonsoft.Json;

namespace PholioVisualisation.PdfData
{
    public class HealthProfilesSupportingInformation : PdfSupportingInformation
    {
        [JsonProperty(PropertyName = "Data")]
        public HealthProfilesData HealthProfilesData { get; set; }

        [JsonProperty(PropertyName = "Content")]
        public HealthProfilesContent HealthProfilesContent { get; set; }
    }
}