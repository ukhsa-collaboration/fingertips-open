
using System.Web;
using Newtonsoft.Json;
using PholioVisualisation.RequestParameters;
using PholioVisualisation.SearchQuerying;

namespace PholioVisualisation.Services
{
    public class JsonpBuilderAreaLookup : JsonBuilderBase
    {
        private readonly AreaLookupParameters _parameters;

        public JsonpBuilderAreaLookup(HttpContextBase context)
            : base(context)
        {
            _parameters = new AreaLookupParameters(context.Request.Params);
            Parameters = _parameters;
        }

        public override string GetJson()
        {
            var placePostcodes = new GeographicalSearch
            {
                AreEastingAndNorthingRetrieved = _parameters.AreEastingAndNorthingRequired,
                ExcludeCcGs = _parameters.ExcludeCcGs
            }.SearchPlacePostcodes(_parameters.SearchText, _parameters.PolygonAreaTypeId);

            var json = JsonConvert.SerializeObject(placePostcodes);
            return _parameters.Callback + "(" + json + ")";
        }
    }
}