using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using System.Linq;

namespace PholioVisualisation.Export
{
    public class ExportPeriodHelper
    {
        public static TimePeriod GetPopulationTimePeriod()
        {
            var groupDataReader = ReaderFactory.GetGroupDataReader();
            var grouping = groupDataReader.GetGroupingsByGroupIdAndIndicatorId( GroupIds.Population, IndicatorIds.QuinaryPopulations);
            var metadataList = IndicatorMetadataProvider.Instance.GetIndicatorMetadataCollection(grouping).IndicatorMetadata;

            var metadata = metadataList.First();
            var timePeriods = grouping.GetTimePeriodIterator(metadata.YearType).TimePeriods;

            return timePeriods.FirstOrDefault();
        }
    }
}
