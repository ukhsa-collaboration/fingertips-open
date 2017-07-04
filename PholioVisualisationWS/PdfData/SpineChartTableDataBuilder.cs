using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PdfData
{
    public class SpineChartTableDataBuilder : DomainDataBuilder
    {
        private SpineChartTableRowDataBuilder rowBuilder;

        private SpineChartTableData spineChartTableData;

        public List<SpineChartTableData> GetDomainDataForProfile(SpineChartDataParameters parameters)
        {
            var profileId = parameters.ProfileId;
            var profileConfig = profileReader.GetProfileConfig(profileId);

            rowBuilder = new SpineChartTableRowDataBuilder(profileId, parameters.AreaCodes)
            {
                IncludeTrendMarkers = profileConfig.HasTrendMarkers && parameters.IncludeRecentTrends
            };

            return BuildDomainDataForProfile(profileId, parameters.ChildAreaTypeId, parameters.BenchmarkAreaCodes)
                .ConvertAll(x => (SpineChartTableData)x);
        }

        protected override DomainData NewDomainData()
        {
            spineChartTableData = new SpineChartTableData();
            return spineChartTableData;
        }

        protected override void AddIndicatorData(GroupRoot groupRoot, IndicatorMetadata metadata, IList<IArea> benchmarkAreas)
        {
            spineChartTableData.IndicatorData.Add(rowBuilder.GetIndicatorData(groupRoot, metadata, benchmarkAreas));
        }
    }
}
