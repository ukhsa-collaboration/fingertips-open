using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    public class JsonBuilderAreaAddress : JsonBuilderBase
    {

        private AreaAddressParameters _parameters;

        public JsonBuilderAreaAddress(HttpContextBase context)
            : base(context)
        {
            _parameters = new AreaAddressParameters(context.Request.Params);
            Parameters = _parameters;
        }

        public JsonBuilderAreaAddress(AreaAddressParameters parameters)
        {
            _parameters = parameters;
            Parameters = _parameters;
        }

        public override string GetJson()
        {
            var area = GetAreaAddress();
            return JsonConvert.SerializeObject(area);
        }

        public AreaAddress GetAreaAddress()
        {
            AreaAddress area = ReaderFactory.GetAreasReader().GetAreaWithAddressFromCode(_parameters.AreaCode);
            area.CleanAddress();
            area.LatitudeLongitude = MapCoordinateConverter.GetLatitudeLongitude(area.EastingNorthing);
            return area;
        }
    }
}