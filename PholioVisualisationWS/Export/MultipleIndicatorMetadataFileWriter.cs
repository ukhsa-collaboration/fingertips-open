using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export
{
    public class MultipleIndicatorMetadataFileWriter
    {
        private HtmlCleaner _htmlCleaner = new HtmlCleaner();

        public byte[] GetMetadataFileAsBytes(IList<IndicatorMetadata> indicatorMetadataList,
            IList<IndicatorMetadataTextProperty> properties)
        {
            // Create CSV writer
            var csvWriter = new CsvWriter();

            // Header
            AddHeader(properties, csvWriter);

            // Add descriptive metadata properties
            foreach (var indicatorMetadata in indicatorMetadataList)
            {
                var textList = new List<string>();

                textList.Add(indicatorMetadata.IndicatorId.ToString());

                AddIndicatorMetadataTextProperties(properties, indicatorMetadata, textList);

                // Add other properties
                textList.Add(indicatorMetadata.Unit.Label);
                textList.Add(indicatorMetadata.ValueType.Name);
                textList.Add(indicatorMetadata.YearType.Name);

                csvWriter.AddLine(textList.Cast<object>().ToArray());
            }

            var bytes = csvWriter.WriteAsBytes();
            return bytes;
        }

        private void AddIndicatorMetadataTextProperties(IList<IndicatorMetadataTextProperty> properties, 
            IndicatorMetadata indicatorMetadata, List<string> textList)
        {
            var descriptive = indicatorMetadata.Descriptive;
            foreach (var property in properties)
            {
                var columnName = property.ColumnName;
                if (descriptive.ContainsKey(columnName))
                {
                    var text = descriptive[columnName];

                    if (string.IsNullOrWhiteSpace(text) == false)
                    {
                        text = _htmlCleaner.RemoveHtml(text);
                        textList.Add(text);
                    }
                    else
                    {
                        // Property whitespace for the indicator
                        textList.Add(text);
                    }
                }
                else
                {
                    // Property not defined for the indicator
                    textList.Add(string.Empty);
                }
            }
        }

        private static void AddHeader(IList<IndicatorMetadataTextProperty> properties, CsvWriter csvWriter)
        {
            var headers = new List<string>();

            headers.Add("Indicator ID");
            headers.AddRange(properties.Select(x => x.DisplayName));
            headers.AddRange(new List<string> {"Unit", "Value type", "Year type"});

            csvWriter.AddHeader(headers.Cast<object>().ToArray());
        }
    }
}