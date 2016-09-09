using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class TrendMarkerWriter
    {
        private CsvWriter _csvWriter;

        public TrendMarkerWriter()
        {
            _csvWriter = new CsvWriter();
            _csvWriter.AddHeader("AreaCode", "Year", "Value", "Denominator", "Count",
                "ChiSquare", "Slope", "Intercept", "Marker", "NumberOfPointsUsedInCalculation", "Message");
        }

        public void Write(TrendRequest trendRequest, TrendMarkerResult trendMarkerResult)
        {
            foreach (var data in trendRequest.Data)
            {
                _csvWriter.AddLine(data.AreaCode, data.Year, data.Value, data.Denominator, data.Count);
            }
            _csvWriter.AddLine("", "", "", "", "", trendMarkerResult.ChiSquare, trendMarkerResult.Slope, trendMarkerResult.Intercept,
                trendMarkerResult.Marker, trendMarkerResult.NumberOfPointsUsedInCalculation, trendMarkerResult.Message);
        }

        public void WriteFile(GroupRoot groupRoot, IArea subnationalArea)
        {
            var bytes = _csvWriter.WriteAsBytes();
            var filename = @"c:\temp\" + groupRoot.IndicatorId + "-sex" + groupRoot.SexId + "-age" + groupRoot.AgeId + "-" + subnationalArea.ShortName + ".csv";
            System.IO.File.WriteAllText(filename, System.Text.Encoding.UTF8.GetString(bytes));
        }
    }
}