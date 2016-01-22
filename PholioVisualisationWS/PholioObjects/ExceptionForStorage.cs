using System;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class ExceptionForStorage
    {
        [JsonProperty]
        public int Id { get; set; }
 
        [JsonProperty]
        public string Environment { get; set; }

        [JsonProperty]
        public string Application { get; set; }

        [JsonProperty]
        public DateTime Date { get; set; }

        [JsonProperty]
        public string UserName { get; set; }

        [JsonProperty]
        public string Message { get; set; }

        [JsonProperty]
        public string StackTrace { get; set; }

        [JsonProperty]
        public string Type { get; set; }

        [JsonProperty]
        public string Url { get; set; }

        [JsonProperty]
        public string Server { get; set; }
    }
}
