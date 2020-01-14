using System.Collections.Generic;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export.FileBuilder.Writers
{
    public class CsvBuilderIndicatorDataHeaderWriter : IBuilderHeaderWriter
    {
        private readonly IAreasReader _areasReader;
        private readonly IndicatorExportParameters _generalParameters;

        public CsvBuilderIndicatorDataHeaderWriter(IAreasReader areasReader, IndicatorExportParameters generalParameters)
        {
            _areasReader = areasReader;
            _generalParameters = generalParameters;
        }
        /// <summary>
        /// Gets the column headers in CSV format
        /// </summary>
        public byte[] GetHeader()
        {
            var csvWriter = new CsvWriter();

            var headings = new List<object>
                {
                    "Indicator ID", "Indicator Name", "Parent Code", "Parent Name",
                    "Area Code", "Area Name", "Area Type", "Sex", "Age",
                    "Category Type", "Category",
                    "Time period", "Value",
                    "Lower CI 95.0 limit", "Upper CI 95.0 limit",
                    "Lower CI 99.8 limit", "Upper CI 99.8 limit",
                    "Count", "Denominator",
                    "Value note", "Recent Trend",
                    "Compared to England value or percentiles",
                    GetComparedToSubNationalParentHeading(),
                    "Time period Sortable",
                    "New data",
                    "Compared to goal"
                };

            csvWriter.AddHeader(headings.ToArray());
            var bytes = csvWriter.WriteAsBytes();
            return bytes;
        }

        private string GetComparedToSubNationalParentHeading()
        {
            var areaType = AreaTypeFactory.New(_areasReader, _generalParameters.ParentAreaTypeId);

            var heading = areaType.Id == AreaTypeIds.Country
                ? "Compared to percentiles"
                : "Compared to " + areaType.ShortName + " value or percentiles";
            return heading;
        }
    }
}
