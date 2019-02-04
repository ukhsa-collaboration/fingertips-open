using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export.FileBuilder.Wrappers
{
    public class CsvBuilderAttributesForIndicatorsWrapper
    {
        public SingleGroupingProvider SingleGroupingProvider { get; private set; }
        public IndicatorMetadataTextOptions IndicatorMetadataTextOption { get; private set; }

        public CsvBuilderAttributesForIndicatorsWrapper(IGroupDataReader groupDataReader, int profileId)
        {
            SingleGroupingProvider = new SingleGroupingProvider(groupDataReader, null);
            IndicatorMetadataTextOption = GetIndicatorMetadataTextOption(profileId);
        }

        private static IndicatorMetadataTextOptions GetIndicatorMetadataTextOption(int profileId)
        {
            return profileId == ProfileIds.Undefined || profileId == ProfileIds.Search
                ? IndicatorMetadataTextOptions.UseGeneric
                : IndicatorMetadataTextOptions.OverrideGenericWithProfileSpecific;
        }
    }
}
