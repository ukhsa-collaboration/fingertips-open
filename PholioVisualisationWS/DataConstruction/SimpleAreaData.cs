using System.Collections.Generic;
using Newtonsoft.Json;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    /// <summary>
    /// DTO for AreaDataBuilder
    /// </summary>
    public class SimpleAreaData
    {
        [JsonProperty("IID")]
        public int IndicatorId { get; set; }

        /// <summary>
        /// Key is parent area code, Value is significance
        /// </summary>
        [JsonProperty("Sig")]
        public Dictionary<string, List<int?>> Significances { get; set; }

        public IList<ValueData> Data { get; set; }
    }
}