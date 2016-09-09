
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
        private TrendDataParameters _parameters;

        public JsonBuilderTrendData(HttpContextBase context)
            : base(context)
        {
            _parameters = new TrendDataParameters(context.Request.Params);
            Parameters = _parameters;
        }

        public JsonBuilderTrendData(TrendDataParameters parameters)
        {
            _parameters = parameters;
            Parameters = _parameters;
        }

        public override string GetJson()
        {
            var trendRoots = GetTrendData();
            return JsonConvert.SerializeObject(trendRoots);
        }

        public IList<TrendRoot> GetTrendData()
        {
            var parentArea = new ParentArea(_parameters.ParentAreaCode, _parameters.AreaTypeId);

            bool isParentCodeNearestNeighbour = Area.IsNearestNeighbour(_parameters.ParentAreaCode);

            var groupData = new GroupDataAtDataPointRepository().GetGroupData(_parameters.ParentAreaCode,
                _parameters.AreaTypeId, _parameters.ProfileId, _parameters.GroupId);

            IList<TrendRoot> trendRoots = new TrendRootBuilder().Build(
               groupData.GroupRoots,
                new ComparatorMapBuilder(parentArea).ComparatorMap,
                _parameters.AreaTypeId,
                _parameters.ProfileId,
                groupData.IndicatorMetadata, isParentCodeNearestNeighbour);

            return trendRoots;
        }
    }
}