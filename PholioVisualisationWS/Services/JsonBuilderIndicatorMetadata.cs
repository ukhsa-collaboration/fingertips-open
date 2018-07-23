using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PholioVisualisation.Services
{
    public class JsonBuilderIndicatorMetadata :JsonBuilderBase
    {
        private IndicatorMetadataParameters _parameters;
        private IGroupDataReader _groupDataReader = ReaderFactory.GetGroupDataReader();
        private IndicatorMetadataProvider _indicatorMetadataProvider = IndicatorMetadataProvider.Instance;

        public JsonBuilderIndicatorMetadata(HttpContextBase context)
            : base(context)
        {
            _parameters = new IndicatorMetadataParameters(context.Request.Params);
            Parameters = _parameters;
        }

        public override string GetJson()
        {
            var metadataMap = GetIndicatorMetadatas();
            return JsonConvert.SerializeObject(metadataMap);
        }

        public JsonBuilderIndicatorMetadata(IndicatorMetadataParameters parameters)
        {
            _parameters = parameters;
        }

        public Dictionary<int, IndicatorMetadata> GetIndicatorMetadatas()
        {
            var indicatorMetadataList = GetIndicatorMetadataList();

            Dictionary<int, IndicatorMetadata> metadataMap = indicatorMetadataList.ToDictionary(
                indicatorMetadata => indicatorMetadata.IndicatorId);

            ModifyPropertiesIfRequired(_indicatorMetadataProvider, indicatorMetadataList);
            _indicatorMetadataProvider.CorrectLocalDocumentLink(indicatorMetadataList);

            return metadataMap;
        }

        private IList<IndicatorMetadata> GetIndicatorMetadataList()
        {
            IList<IndicatorMetadata> indicatorMetadataList;
            if (_parameters.UseIndicatorIds)
            {
                // Indicators displayed in search
                indicatorMetadataList = GetIndicatorMetadataFromIdList();
            }
            else if (_parameters.UseGroupIds)
            {
                // Indicators displayed in profile
                indicatorMetadataList = GetIndicatorMetadataInGroups();
            }
            else
            {
                // All indicators
                var ids = _groupDataReader.GetAllIndicatorIds();
                indicatorMetadataList = _indicatorMetadataProvider.GetIndicatorMetadata(ids);
            }
            return indicatorMetadataList;
        }

        private IList<IndicatorMetadata> GetIndicatorMetadataInGroups()
        {
            IList<Grouping> groupings = _groupDataReader.GetGroupingsByGroupIds(_parameters.GroupIds);
            var indicatorMetadataList = _indicatorMetadataProvider.GetIndicatorMetadata(groupings,
                IndicatorMetadataTextOptions.OverrideGenericWithProfileSpecific);
            return indicatorMetadataList;
        }

        private IList<IndicatorMetadata> GetIndicatorMetadataFromIdList()
        {
            IList<IndicatorMetadata> indicatorMetadataList;
            var profileIds = _parameters.RestrictResultsToProfileIds;

            var indicatorIds = _parameters.IndicatorIds;

            if (profileIds.Any())
            {
                IProfileReader profileReader = ReaderFactory.GetProfileReader();
                IList<Grouping> groupings = new GroupingListProvider(_groupDataReader, profileReader)
                    .GetGroupings(profileIds, indicatorIds);

                var matchedGroupings = groupings
                    .GroupBy(x => x.IndicatorId)
                    .Select(grp => grp.First());

                indicatorMetadataList = _indicatorMetadataProvider.GetIndicatorMetadata(matchedGroupings.ToList(),
                    IndicatorMetadataTextOptions.UseGeneric);
            }
            else
            {
                indicatorMetadataList = _indicatorMetadataProvider.GetIndicatorMetadata(indicatorIds);
            }

            return indicatorMetadataList;
        }

        private void ModifyPropertiesIfRequired(IndicatorMetadataProvider indicatorMetadataProvider, IList<IndicatorMetadata> indicatorMetadataList)
        {
            if (_parameters.IncludeDefinition == false)
            {
                indicatorMetadataProvider.ReduceDescriptiveMetadata(indicatorMetadataList);
            }

            if (_parameters.IncludeSystemContent == false)
            {
                //Remove the system content fields (This is the default)
                indicatorMetadataProvider.RemoveSystemContentFields(indicatorMetadataList);
            }
        }
    }
}