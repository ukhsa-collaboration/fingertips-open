
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
    public class JsonBuilderIndicatorMetadata : JsonBuilderBase
    {
        private IndicatorMetadataParameters parameters;

        public JsonBuilderIndicatorMetadata(HttpContextBase context)
            : base(context)
        {
            parameters = new IndicatorMetadataParameters(context.Request.Params);
            Parameters = parameters;
        }

        public override string GetJson()
        {
            IndicatorMetadataRepository indicatorMetadataRepository = IndicatorMetadataRepository.Instance;
            IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();
            IProfileReader profileReader = ReaderFactory.GetProfileReader();

            IList<IndicatorMetadata> indicatorMetadataList;
            if (parameters.UseIndicatorIds)
            {
                var profileIds = parameters.RestrictResultsToProfileIds;

                var indicatorIds = parameters.IndicatorIds;

                if (profileIds.Any())
                {
                    IList<Grouping> groupings = new GroupingListProvider(groupDataReader, profileReader)
                        .GetGroupings(profileIds, indicatorIds);

                    var matchedGroupings = groupings
                        .GroupBy(x => x.IndicatorId)
                        .Select(grp => grp.First());

                    indicatorMetadataList = indicatorMetadataRepository.GetIndicatorMetadata(matchedGroupings.ToList());
                }
                else
                {
                    indicatorMetadataList = indicatorMetadataRepository.GetIndicatorMetadata(indicatorIds);
                }
            }
            else
            {
                IList<Grouping> groupings = groupDataReader.GetGroupingsByGroupIds(parameters.GroupIds);
                indicatorMetadataList = indicatorMetadataRepository.GetIndicatorMetadata(groupings);
            }

            Dictionary<int, IndicatorMetadata> metadataMap = indicatorMetadataList.ToDictionary(
                indicatorMetadata => indicatorMetadata.IndicatorId);

            if (parameters.IncludeDefinition == false)
            {
                indicatorMetadataRepository.ReduceDescriptiveMetadata(indicatorMetadataList);
            }

            if (parameters.IncludeSystemContent==false)
            {
                //Remove the system content fields (This is the default)
                indicatorMetadataRepository.RemoveSystemContentFields(indicatorMetadataList);
            }
            
            return JsonConvert.SerializeObject(metadataMap);
        }
    }
}
