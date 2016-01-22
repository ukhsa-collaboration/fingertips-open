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

        public List<SpineChartTableData> GetDomainDataForProfile(int profileId, int childAreaTypeId, IList<string> areaCodes, 
            IList<string> benchmarkAreaCodes)
        {
            rowBuilder = new SpineChartTableRowDataBuilder(profileId, areaCodes);

            return BuildDomainDataForProfile(profileId, childAreaTypeId, benchmarkAreaCodes).ConvertAll(x => (SpineChartTableData)x);
        }

        protected override DomainData NewDomainData()
        {
            spineChartTableData = new SpineChartTableData();
            return spineChartTableData;
        }

        protected override void AddIndicatorData(GroupRoot groupRoot, IndicatorMetadata metadata, IList<Area> benchmarkAreas)
        {
            spineChartTableData.IndicatorData.Add(rowBuilder.GetIndicatorData(groupRoot, metadata, benchmarkAreas));
        }
    }
}
