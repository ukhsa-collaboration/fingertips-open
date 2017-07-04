using System.Collections.Generic;

namespace PholioVisualisation.PdfData
{
    public class SpineChartDataParameters
    {
        public int ProfileId;
        public int ChildAreaTypeId;
        public IList<string> AreaCodes;
        public IList<string> BenchmarkAreaCodes;
        public bool IncludeRecentTrends;
    }
}