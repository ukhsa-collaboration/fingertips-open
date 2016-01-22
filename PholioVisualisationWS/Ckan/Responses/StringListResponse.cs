using Newtonsoft.Json;

namespace Ckan.Responses
{
    public class CkanResultResponse<T> : BaseCkanResponse
    {
        [JsonProperty(PropertyName = "result")]
        public T Result { get; set; }
    }
}