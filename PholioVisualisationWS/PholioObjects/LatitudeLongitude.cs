using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class LatitudeLongitude
    {
        [JsonProperty(PropertyName = "Lat")]
        public double Latitude { get; set; }

        [JsonProperty(PropertyName = "Lng")]
        public double Longitude { get; set; }
    }
}
