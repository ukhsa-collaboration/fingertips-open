
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using NHibernate.Mapping;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    public class JsonBuilderAreaData : JsonBuilderBase
    {
        private AreaDataParameters parameters;

        public JsonBuilderAreaData(HttpContextBase context)
            : base(context)
        {
            parameters = new AreaDataParameters(context.Request.Params);
            Parameters = parameters;
        }

        public override string GetJson()
        {
            var areaListBuilder = new AreaListBuilder(ReaderFactory.GetAreasReader());
            areaListBuilder.CreateAreaListFromAreaCodes(parameters.AreaCodes);
            var areas = areaListBuilder.Areas;

            Dictionary<int, object> groupIdToData = new Dictionary<int, object>();

            foreach (var groupId in parameters.GroupIds)
            {
                var data = new AreaDataBuilder
                {
                    GroupId = groupId,
                    Areas = areas,
                    AreaTypeId = parameters.AreaTypeId,
                    ComparatorAreaCodes = parameters.ComparatorAreaCodes,
                    IncludeTimePeriods = parameters.IncludeTimePeriods,
                    LatestDataOnly = parameters.LatestDataOnly
                }.Build();
                groupIdToData.Add(groupId, data);
            }

            if (parameters.GroupIds.Count == 1)
            {
                // For backwards compatibility do not include group ID in response object
                return JsonConvert.SerializeObject(groupIdToData.Values.First());
            }

            return JsonConvert.SerializeObject(groupIdToData);
        }
    }
}