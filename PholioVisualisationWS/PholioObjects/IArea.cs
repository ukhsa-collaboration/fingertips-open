using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public interface IArea
    {
        [JsonProperty]
        string Code { get; }

        [JsonProperty]
        int? Sequence { get; }

        [JsonProperty]
        string Name { get; }

        [JsonProperty]
        string ShortName { get; }

        [JsonProperty]
        int AreaTypeId { get; }

        [JsonIgnore]
        bool IsCcg { get; }

        [JsonIgnore]
        bool IsGpDeprivationDecile { get; }

        [JsonIgnore]
        bool IsShape { get; }

        [JsonIgnore]
        bool IsCountry { get;}

        [JsonIgnore]
        bool IsCountyAndUADeprivationDecile { get; }

        [JsonIgnore]
        bool IsGpPractice { get; }
    }
}