using Newtonsoft.Json;

namespace Fpm.MainUI.ViewModels.ProfilesAndIndicators
{
    public class GroupingPlusNameViewModel
    {
        [JsonProperty(PropertyName = "IndicatorId")]
        public int IndicatorId { get; set; }

        [JsonProperty(PropertyName = "IndicatorName")]
        public string IndicatorName { get; set; }

        [JsonProperty(PropertyName = "SexId")]
        public int SexId { get; set; }

        [JsonProperty(PropertyName = "Sex")]
        public string Sex { get; set; }

        [JsonProperty(PropertyName = "AgeId")]
        public int AgeId { get; set; }

        [JsonProperty(PropertyName = "Age")]
        public string Age { get; set; }

        [JsonProperty(PropertyName = "Sequence")]
        public int Sequence { get; set; }

        [JsonProperty(PropertyName = "GroupId")]
        public int GroupId { get; set; }
    }
}