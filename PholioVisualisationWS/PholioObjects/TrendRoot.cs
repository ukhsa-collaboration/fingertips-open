using System.Collections.Generic;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class TrendRoot : RootBase
    {
        public TrendRoot(GroupRoot rootToCopy)
        {
            IndicatorId = rootToCopy.IndicatorId;
            StateSex = rootToCopy.StateSex;
            AreaTypeId = rootToCopy.AreaTypeId;
            SexId = rootToCopy.SexId;
            AgeId = rootToCopy.AgeId;
            PolarityId = rootToCopy.PolarityId;
            ComparatorMethodId = rootToCopy.ComparatorMethodId;
            DataPoints = new Dictionary<string, IList<TrendDataPoint>>();
            ComparatorValueFs = new List<Dictionary<int, string>>();
            ComparatorValue = new List<Dictionary<int, double>>();
            Periods = new List<string>();
        }

        [JsonProperty]
        public Limits Limits { get; set; }

        [JsonProperty(PropertyName = "Data")]
        public Dictionary<string, IList<TrendDataPoint>> DataPoints { get; private set; }

        [JsonProperty(PropertyName = ("ComparatorValueFs"))]
        public List<Dictionary<int, string>> ComparatorValueFs { get; private set; }

        [JsonProperty(PropertyName = ("ComparatorValue"))]
        public List<Dictionary<int, double>> ComparatorValue { get; private set; }

        [JsonProperty(PropertyName = "Periods")]
        public IList<string> Periods { get; set; }

    }
}
