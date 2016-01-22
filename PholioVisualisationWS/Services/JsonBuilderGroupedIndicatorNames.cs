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
    /// <summary>
    /// Service to provide all indicator names & ids.
    /// Intended to enable as-you-type indicator name filtering of a menu of
    /// all indicators within a profile.
    /// </summary>
    public class JsonBuilderGroupedIndicatorNames : JsonBuilderBase
    {
        private GroupedIndicatorNamesParameters parameters;

        public JsonBuilderGroupedIndicatorNames(HttpContextBase context)
            : base(context)
        {
            parameters = new GroupedIndicatorNamesParameters(context.Request.Params);
            Parameters = parameters;
        }

        public override string GetJson()
        {
            var reader = ReaderFactory.GetGroupDataReader();
            IndicatorMetadataRepository indicatorMetadataRepository = IndicatorMetadataRepository.Instance;

            Dictionary<int, object> groupIdToIndicatorMap = new Dictionary<int, object>();

            foreach (var groupId in parameters.GroupIds)
            {
                IList<Grouping> groupings = reader.GetGroupingsByGroupId(groupId);

                var indicatorMetadataList = indicatorMetadataRepository.GetIndicatorMetadata(groupings)
                    .Select(x => new
                    {
                        IID = x.IndicatorId,
                        Name = x.Descriptive[IndicatorMetadataTextColumnNames.Name]
                    });

                groupIdToIndicatorMap.Add(groupId, indicatorMetadataList.ToList());
            }

            return JsonConvert.SerializeObject(groupIdToIndicatorMap);
        }
    }
}