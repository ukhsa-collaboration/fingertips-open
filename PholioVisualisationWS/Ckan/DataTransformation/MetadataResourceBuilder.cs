using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ckan.Model;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace Ckan.DataTransformation
{
    public class MetadataResourceBuilder
    {
        private HtmlCleaner htmlCleaner = new HtmlCleaner();

        public CkanResource GetUnsavedResource(string packageId, IndicatorMetadata indicatorMetadata,
            IList<IndicatorMetadataTextProperty> properties)
        {
            var descriptive = indicatorMetadata.Descriptive;
            var indicatorName = descriptive[IndicatorMetadataTextColumnNames.Name];

            // Add metadata resource
            var resource = new CkanResource();
            resource.PackageId = packageId;
            resource.Description = "Metadata for \"" + indicatorName + "\"";
            resource.Format = "CSV";
            resource.Name = "Metadata";

            // Add file to resource
            var fileContents = GetMetadataFileAsBytes(indicatorMetadata, properties, descriptive);
            var fileNamer = new CkanFileNamer(indicatorName);
            resource.File = new CkanResourceFile
            {
                FileName = fileNamer.MetadataFileName,
                FileContents = fileContents
            };

            return resource;
        }

        private byte[] GetMetadataFileAsBytes(IndicatorMetadata indicatorMetadata, 
            IList<IndicatorMetadataTextProperty> properties, IDictionary<string, string> descriptive)
        {
            // Create CSV writer
            var csvWriter = new CsvWriter();
            csvWriter.AddHeader("Field", "Text");

            // Add descriptive metadata properties
            foreach (var property in properties)
            {
                if (descriptive.ContainsKey(property.ColumnName))
                {
                    var text = descriptive[property.ColumnName];

                    if (string.IsNullOrWhiteSpace(text) == false)
                    {
                        text = htmlCleaner.RemoveHtml(text);
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