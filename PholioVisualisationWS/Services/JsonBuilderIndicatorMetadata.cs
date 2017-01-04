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
    public class JsonBuilderIndicatorMetadata : JsonBuilderBase
    {
        private IndicatorMetadataParameters _parameters;

        public JsonBuilderIndicatorMetadata(HttpContextBase context)
            : base(context)
        {
            _parameters = new IndicatorMetadataParameters(context.Request.Params);
            Parameters = _parameters;
        }

        public JsonBuilderIndicatorMetadata(IndicatorMetadataParameters parameters)
        {
            _parameters = parameters;
            Parameters = _parameters;
        }

        public override string GetJson()
        {
            var metadataMap = GetIndicatorMetadatas();

            //Modify
            string result = JsonConvert.SerializeObject(metadataMap);

            //TODO replace href="document/\ with full url
            var uiUrl = ApplicationConfiguration.UrlUI; //   will get http://localhost:59822\

            result = result.Replace(@"documents\PHOF_Overarching_user_guide_February_2016_final.docx", uiUrl+ @"\documents\PHOF_Overarching_user_guide_February_2016_final.docx");
            return result;
            //return JsonConvert.SerializeObject(metadataMap);
        }

        public Dictionary<int, IndicatorMetadata> GetIndicatorMetadatas()
        {
            IndicatorMetadataProvider indicatorMetadataProvider = IndicatorMetadataProvider.Instance;
            IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();
            IProfileReader profileReader = ReaderFactory.GetProfileReader();

            IList<IndicatorMetadata> indicatorMetadataList;
            if (_parameters.UseIndicatorIds)
            {
                // Indicators displayed in search
                var profileIds = _parameters.RestrictResultsToProfileIds;

                var indicatorIds = _parameters.IndicatorIds;

                if (profileIds.Any())
                {
                    IList<Grouping> groupings = new GroupingListProvider(groupDataReader, profileReader)
                        .GetGroupings(profileIds, indicatorIds);

                    var matchedGroupings = groupings
                        .GroupBy(x => x.IndicatorId)
                        .Select(grp => grp.First());

                    indicatorMetadataList = indicatorMetadataProvider.GetIndicatorMetadata(matchedGroupings.ToList(),
                        IndicatorMetadataTextOptions.UseGeneric);
                }
                else
                {
                    indicatorMetadataList = indicatorMetadataProvider.GetIndicatorMetadata(indicatorIds);
                }
            }
            else
            {
                // Indicators displayed in profile
                IList<Grouping> groupings = groupDataReader.GetGroupingsByGroupIds(_parameters.GroupIds);
                indicatorMetadataList = indicatorMetadataProvider.GetIndicatorMetadata(groupings,
                    IndicatorMetadataTextOptions.OverrideGenericWithProfileSpecific);
            }

            Dictionary<int, IndicatorMetadata> metadataMap = indicatorMetadataList.ToDictionary(
                indicatorMetadata => indicatorMetadata.IndicatorId);

            if (_parameters.IncludeDefinition == false)
            {
                indicatorMetadataProvider.ReduceDescriptiveMetadata(indicatorMetadataList);
            }

            if (_parameters.IncludeSystemContent == false)
            {
                //Remove the system content fields (This is the default)
                indicatorMetadataProvider.RemoveSystemContentFields(indicatorMetadataList);
            }


            //TODO replace href="document/\ with full url
            //var uiUrl = ApplicationConfiguration.UrlUI;


            indicatorMetadataProvider.CorrectLocalDocumentLink(indicatorMetadataList);


            return metadataMap;
        }
    }
}