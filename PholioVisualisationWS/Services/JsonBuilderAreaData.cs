
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    public class JsonBuilderAreaData 
    {
        private AreaDataParameters _parameters;

        public JsonBuilderAreaData(AreaDataParameters parameters)
        {
            _parameters = parameters;
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