
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class SparklineArea
    {
        [JsonProperty]
        public IList<SparklineTimePoint> TimePoints { get; set; }

        public SparklineArea()
        {
            TimePoints = new List<SparklineTimePoint>();
        }
    }
}
