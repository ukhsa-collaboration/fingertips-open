
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    public class JsonBuilderGroupDataAtTimePoint : JsonBuilderBase
    {
        private GroupData data;
        private GroupDataAtTimePointParameters parameters;

        public JsonBuilderGroupDataAtTimePoint(HttpContextBase context)
            : base(context)
        {
            parameters = new GroupDataAtTimePointParameters(context.Request.Params);
            Parameters = parameters;
        }

        public override string GetJson()
        {
            var parentArea = new ParentArea(parameters.ParentAreaCode, parameters.AreaTypeId);

            // As JSON will be cached but putting objects in repository not useful
            data = new GroupDataBuilderByGroupings
            {
                GroupId = parameters.GroupId,
                ProfileId = parameters.ProfileId,
                ComparatorMap = new ComparatorMapBuilder(parentArea).ComparatorMap,
                TimePeriodOfData = parameters.TimePeriod,
                ChildAreaTypeId = parameters.AreaTypeId
            }.Build();

            new GroupDataProcessor().Process(data,
                new SpecifiedTimePeriodFormatter { TimePeriod = parameters.TimePeriod });

            return new JsonBuilderGroupData
            {
                GroupRoots = data.GroupRoots,
                IndicatorMetadata = data.IndicatorMetadata
            }.BuildJson();
        }

    }
}