using System.Collections.Generic;
using Newtonsoft.Json;

namespace Ckan.Responses
{
    public class CkanErrorResponse : BaseCkanResponse
    {
        [JsonProperty(PropertyName = "error")]
        public Dictionary<string,object> ErrorDictionary { get; set; }
    }
}