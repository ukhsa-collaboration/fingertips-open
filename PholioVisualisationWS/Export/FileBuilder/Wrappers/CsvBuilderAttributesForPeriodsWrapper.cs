using System.Collections.Generic;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;

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
