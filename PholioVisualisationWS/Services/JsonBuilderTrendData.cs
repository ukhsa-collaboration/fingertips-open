
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    public class JsonBuilderTrendData : JsonBuilderBase
    {
        private TrendDataParameters parameters;

        public JsonBuilderTrendData(HttpContextBase context)
            : base(context)
        {
            parameters = new TrendDataParameters(context.Request.Params);
            Parameters = parameters;
        }

        public override string GetJson()
        {
            var parentArea = new ParentArea(parameters.ParentAreaCode, parameters.AreaTypeId);
           
            bool isParentCodeNearestNeighbour = Area.IsNearestNeighbour(parameters.ParentAreaCode);

            var groupData = new GroupDataAtDataPointRepository().GetGroupData(parameters.ParentAreaCode,
                parameters.AreaTypeId, parameters.ProfileId,parameters.GroupId);

            IList<TrendRoot> trendRoots = new TrendRootBuilder().Build(
               groupData.GroupRoots,
                new ComparatorMapBuilder(parentArea).ComparatorMap,
                parameters.AreaTypeId,
                parameters.ProfileId,
                groupData.IndicatorMetadata, isParentCodeNearestNeighbour);

            return JsonConvert.SerializeObject(trendRoots);
        }
    }
}