using System.Collections.Generic;

namespace PholioVisualisation.PdfData
{
    public class LocalAndEnglandChartDataWithDeprivation : LocalAndEnglandChartData
    {
        public LocalAndEnglandChartDataWithDeprivation(LocalAndEnglandChartData data)
        {
            England = data.England;
            Local = data.Local;
        }

        public List<double> LocalLeastDeprived { get; set; }
        public List<double> LocalMostDeprived { get; set; }
    }
}