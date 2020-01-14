using PholioVisualisation.Analysis.TrendMarkers;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using System.Collections.Generic;

namespace PholioVisualisation.Export.FileBuilder.Wrappers
{
    public class CsvBuilderAttributesForPeriodsWrapper
    {
        public TrendMarkersProvider TrendMarkersProvider { get; private set; }
        public IList<int> ExcludedCategoryTypeIds { get; private set; }

        public CsvBuilderAttributesForPeriodsWrapper()
        {
            TrendMarkersProvider = new TrendMarkersProvider(ReaderFactory.GetTrendDataReader(), new TrendMarkerCalculator());
            ExcludedCategoryTypeIds = CategoryTypeIdsExcludedForExport.Ids;
        }
    }
}