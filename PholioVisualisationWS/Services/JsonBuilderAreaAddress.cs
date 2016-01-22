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

        private AreaAddressParameters parameters;

        public JsonBuilderAreaAddress(HttpContextBase context)
            : base(context)
        {
            parameters = new AreaAddressParameters(context.Request.Params);
            Parameters = parameters;
        }

        public override string GetJson()
        {
            AreaAddress area = ReaderFactory.GetAreasReader().GetAreaWithAddressFromCode(parameters.AreaCode);
            area.CleanAddress();
            area.LatitudeLongitude = MapCoordinateConverter.GetLatitudeLongitude(area.EastingNorthing);
            return JsonConvert.SerializeObject(area);
        }
    }
}