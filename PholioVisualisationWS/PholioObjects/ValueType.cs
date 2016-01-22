
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class ValueType
    {
        [JsonProperty]
        public int Id { get; set; }
        [JsonProperty]
        public string Label { get; set; }
    }
}
