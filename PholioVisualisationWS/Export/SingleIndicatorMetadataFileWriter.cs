using System.Collections.Generic;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export
{
    public class SingleIndicatorMetadataFileWriter
    {
        private HtmlCleaner _htmlCleaner = new HtmlCleaner();

        public byte[] GetMetadataFileAsBytes(IndicatorMetadata indicatorMetadata,
            IList<IndicatorMetadataTextProperty> properties)
        {
            // Create CSV writer
            var csvWriter = new CsvWriter();
            csvWriter.AddHeader("Field", "Text");

            // Add descriptive metadata properties
            var descriptive = indicatorMetadata.Descriptive;
            foreach (var property in properties)
            {
                if (descriptive.ContainsKey(property.ColumnName))
                {
                    var text = descriptive[property.ColumnName];

                    if (string.IsNullOrWhiteSpace(text) == false)
                    {
                        text = _htmlCleaner.RemoveHtml(text);
                        csvWriter.AddLine(property.DisplayName, text);
                    }
                }
            }

            // Add other properties
            csvWriter.AddLine("Unit", indicatorMetadata.Unit.Label);
            csvWriter.AddLine("Value type", indicatorMetadata.ValueType.Name);
            csvWriter.AddLine("Year type", indicatorMetadata.YearType.Name);

            var bytes = csvWriter.WriteAsBytes();
            return bytes;
        }
    }
}