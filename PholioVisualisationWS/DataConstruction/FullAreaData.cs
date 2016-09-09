using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    /// <summary>
    /// DTO for AreaDataBuilder.
    /// </summary>
    public class FullAreaData : SimpleAreaData
    {
        public bool StateSex { get; set; }
        public Sex Sex { get; set; }           
        public Age Age { get; set; }           

        [JsonProperty]
        public int ComparatorMethodId { get; set; }    
       
        [JsonProperty("SigLevel")]
        public double ComparatorConfidence { get; set; }

        public IList<string> Periods { get; set; }
    }
}