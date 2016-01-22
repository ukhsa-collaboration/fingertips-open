using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PholioVisualisation.DataConstruction
{
    /// <summary>
    /// DTO for AreaDataBuilder.
    /// </summary>
    public class FullAreaData : SimpleAreaData
    {
        public bool StateSex { get; set; }
        public int SexId { get; set; }           
        public int AgeId { get; set; }           

        [JsonProperty("MethodId")]
        public int ComparatorMethodId { get; set; }    
       
        [JsonProperty("SigLevel")]
        public double ComparatorConfidence { get; set; }

        public IList<string> Periods { get; set; }
    }
}