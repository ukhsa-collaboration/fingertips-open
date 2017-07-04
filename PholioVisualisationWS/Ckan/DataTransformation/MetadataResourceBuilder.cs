using System.Collections.Generic;
using System.Linq;
using Ckan.Model;
using PholioVisualisation.Export;
using PholioVisualisation.Export.File;
using PholioVisualisation.PholioObjects;

namespace Ckan.DataTransformation
{
    public class MetadataResourceBuilder
    {
        public CkanResource GetUnsavedResource(string packageId, IndicatorMetadata indicatorMetadata,
            IList<IndicatorMetadataTextProperty> properties)
        {
            var indicatorName = indicatorMetadata.Name;

            // Add metadata resource
            var resource = new CkanResource();
            resource.PackageId = packageId;
            resource.Description = "Metadata for \"" + indicatorName + "\"";
            resource.Format = "CSV";
            resource.Name = "Metadata";

            // Add file to resource
            var fileContents = new SingleIndicatorMetadataFileWriter()
                .GetMetadataFileAsBytes(indicatorMetadata, properties);
            var fileNamer = new SingleEntityFileNamer(indicatorName);
            resource.File = new CkanResourceFile
            {
                FileName = fileNamer.MetadataFileName,
                FileContents = fileContents
            };

            return resource;
        }
    }
}