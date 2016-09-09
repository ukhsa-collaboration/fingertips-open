
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    public class JsonBuilderAreaData : JsonBuilderBase
    {
        private AreaDataParameters _parameters;

        public JsonBuilderAreaData(HttpContextBase context)
            : base(context)
        {
            _parameters = new AreaDataParameters(context.Request.Params);
            Parameters = _parameters;
        }

        public JsonBuilderAreaData(AreaDataParameters parameters)
        {
            _parameters = parameters;
            Parameters = _parameters;
        }

        public override string GetJson()
        {
            // IMPORTANT : must include if (_parameters.GroupIds.Count == 1) section when wrapping this method
            var groupIdToData = GetAreaData();

            if (_parameters.GroupIds.Count == 1)
            {
                // For backwards compatibility do not include group ID in response object
                var data = groupIdToData.Values.First();
                return JsonConvert.SerializeObject(data);
            }

            return JsonConvert.SerializeObject(groupIdToData);
        }

        public Dictionary<int, Dictionary<string, IList<SimpleAreaData>>> GetAreaData()
        {
            var areaListBuilder = new AreaListProvider(ReaderFactory.GetAreasReader());
            areaListBuilder.CreateAreaListFromAreaCodes(_parameters.AreaCodes);
            var areas = areaListBuilder.Areas;

            Dictionary<int, Dictionary<string, IList<SimpleAreaData>>> groupIdToData = 
                new Dictionary<int, Dictionary<string, IList<SimpleAreaData>>>();

            foreach (var groupId in _parameters.GroupIds)
            {
                Dictionary<string, IList<SimpleAreaData>> data = new AreaDataBuilder
                {
                    GroupId = groupId,
                    Areas = areas,
                    AreaTypeId = _parameters.AreaTypeId,
                    ComparatorAreaCodes = _parameters.ComparatorAreaCodes,
                    IncludeTimePeriods = _parameters.IncludeTimePeriods,
                    LatestDataOnly = _parameters.LatestDataOnly
                }.Build();
                groupIdToData.Add(groupId, data);
            }

            return groupIdToData;
        }
    }
}