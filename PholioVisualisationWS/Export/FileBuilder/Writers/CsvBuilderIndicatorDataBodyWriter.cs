using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export.File;
using PholioVisualisation.Export.FileBuilder.Containers;

namespace PholioVisualisation.Export.FileBuilder.Writers
{ 
    public class CsvBuilderIndicatorDataBodyWriter : IBuilderBodyWriter<byte[]>
    {
        private readonly IndicatorDataBodyContainer _indicatorDataBodyContainer;
        public readonly CsvBuilderAttributesForBodyContainer AttributesForBodyContainer;

        public CsvBuilderIndicatorDataBodyWriter(IndicatorMetadataProvider indicatorMetadataProvider, ExportAreaHelper areaHelper, IAreasReader areasReader, CsvBuilderAttributesForBodyContainer attributesForBodyContainer)
        {
            _indicatorDataBodyContainer = new IndicatorDataBodyContainer(indicatorMetadataProvider, areaHelper, areasReader, attributesForBodyContainer);
            AttributesForBodyContainer = attributesForBodyContainer;
        }

        public byte[] GetBody()
        {
            var fileBuilderBody = new CsvFileBuilder();

            foreach (var indicatorId in AttributesForBodyContainer.OnDemandParameters.IndicatorIds)
            {
                // Add file contents
                var indicatorFileContents = _indicatorDataBodyContainer.GetIndicatorDataFile(indicatorId);
                fileBuilderBody.AddContent(indicatorFileContents);
            }

            return fileBuilderBody.GetFileContent();
        }
    }
}
