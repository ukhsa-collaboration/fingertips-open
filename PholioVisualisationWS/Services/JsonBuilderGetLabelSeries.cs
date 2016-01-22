
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
    public class JsonBuilderGetLabelSeries : JsonBuilderBase
    {
        private LabelSeriesParameters parameters;

        public JsonBuilderGetLabelSeries(HttpContextBase context)
            : base(context)
        {
            parameters = new LabelSeriesParameters(context.Request.Params);
            Parameters = parameters;
        }

        public override string GetJson()
        {
            string id = parameters.SeriesId.ToLower();

            Dictionary<string, object> responseObjects = new Dictionary<string, object>();

            object labels;
            if (id == "qpop")
            {
                labels = ReaderFactory.GetPholioReader().GetQuinaryPopulationLabels();
            }
            else if (id == "dep")
            {
                labels = new AreaLabelsBuilder(ReaderFactory.GetAreasReader().GetCategories(CategoryTypeIds.DeprivationDecileGp2015)).Labels;                    
            }
            else if (id == "shape")
            {
                labels = new AreaLabelsBuilder(
                     ReaderFactory.GetAreasReader().GetAreasByAreaTypeId(AreaTypeIds.Shape),
                     AreaLabelsBuilder.ShapeAreaCodePrefix).Labels;
            }
            else
            {
                labels = new List<string>();
            }

            responseObjects.Add("Labels", labels);
            responseObjects.Add("Id", parameters.SeriesId);

            return JsonConvert.SerializeObject(responseObjects);
        }
    }
}