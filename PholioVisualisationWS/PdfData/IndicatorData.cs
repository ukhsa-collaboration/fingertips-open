using System.Collections.Generic;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PdfData
{
    public class IndicatorData
    {
        public int IndicatorId { get; set; }
        public string ShortName { get; set; }
        public string IndicatorNumber { get; set; }
        public int SexId { get; set; }
        public int AgeId { get; set; }
        public int PolarityId { get; set; }
        public string Period { get; set; }
        public IDictionary<string, CoreDataSet> BenchmarkData { get; set; }
    }
}