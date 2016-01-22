
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    public class JsonBuilderGetLabel : JsonBuilderBase
    {
        private LabelParameters parameters;
        private Dictionary<string, object> labels = new Dictionary<string, object>();


        public JsonBuilderGetLabel(HttpContextBase context)
            : base(context)
        {
            parameters = new LabelParameters(context.Request.Params);
            Parameters = parameters;
        }

        public override string GetJson()
        {
            int id;
            PholioLabelReader reader = new PholioLabelReader();

            labels.Add(LabelParameters.ParameterAjaxKey, parameters.AjaxKey.ToString());

            if (parameters.IsAgeIdValid)
            {
                id = parameters.AgeId;
                AddLabel(ParameterNames.AgeId, id, reader.LookUpAgeLabel(id));
            }

            if (parameters.IsYearTypeIdValid)
            {
                id = parameters.YearTypeId;
                AddLabel(ParameterNames.YearTypeId, id, reader.LookUpYearTypeLabel(id));
            }

            if (parameters.IsComparatorMethodIdValid)
            {
                id = parameters.ComparatorMethodId;
                AddLabel(LabelParameters.ParameterComparatorMethod, id, reader.LookUpComparatorMethodLabel(id));
            }

            if (parameters.IsConfidenceIntervalMethodIdValid)
            {
                id = parameters.ConfidenceIntervalMethodId;
                labels.Add(LabelParameters.ParameterConfidenceIntervalMethod,
                    ReaderFactory.GetGroupDataReader().GetConfidenceIntervalMethod(id));
            }

            return JsonConvert.SerializeObject(labels);
        }

        private void AddLabel(string parameterKey, int id, string labelText)
        {
            labels.Add(parameterKey, new { Id = id, Text = labelText });
        }
    }
}