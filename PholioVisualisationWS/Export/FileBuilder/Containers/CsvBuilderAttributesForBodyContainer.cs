using System.Collections.Generic;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Export.FileBuilder.Wrappers;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export.FileBuilder.Containers
{
    public class CsvBuilderAttributesForBodyContainer
    {
        public IndicatorExportParameters GeneralParameters;
        public OnDemandQueryParametersWrapper OnDemandParameters;
        public CsvBuilderAttributesForIndicatorsWrapper AttributesForIndicators;
        public IGroupDataReader GroupDataReader { get; private set; }

        public CsvBuilderAttributesForBodyContainer(IndicatorExportParameters parameters, OnDemandQueryParametersWrapper onDemandParameters)
        {
            GeneralParameters = parameters;
            OnDemandParameters = onDemandParameters;
            GroupDataReader = ReaderFactory.GetGroupDataReader();
            AttributesForIndicators = new CsvBuilderAttributesForIndicatorsWrapper(GroupDataReader, GeneralParameters.ProfileId);
        }

        public bool HasGrouping(int indicatorId, IList<int> groupIds, out Grouping grouping)
        {
            grouping = null;
            // Clear session to avoid out of memorygroup
            GroupDataReader.ClearSession();

            // Find the grouping with the latest data
            grouping = AttributesForIndicators.SingleGroupingProvider.GetGroupingWithLatestDataPoint(groupIds, indicatorId, 
                GeneralParameters.ChildAreaTypeId, GeneralParameters.ProfileId);
            return grouping != null;
        }
    }
}
