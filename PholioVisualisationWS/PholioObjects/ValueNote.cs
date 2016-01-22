using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class ValueNote
    {
        [JsonProperty]
        public int Id { get; set; }
        
        [JsonProperty]
        public string Text { get; set; }
    }
}
