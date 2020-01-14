using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export.File;
using PholioVisualisation.Export.FileBuilder.Wrappers;
using PholioVisualisation.Export.FileBuilder.Writers;

namespace PholioVisualisation.Export.FileBuilder
{
    public class IndicatorDataCsvFileBuilder
    {
        // Dependencies        
        private IFileBuilder FileBuilder;
        private IFileBuilderWriter FileBuilderWriter;

        public IndicatorDataCsvFileBuilder(IFileBuilder fileBuilder, IFileBuilderWriter fileBuilderWriter)
        {
            FileBuilder = fileBuilder;
            FileBuilderWriter = fileBuilderWriter;
        }

        public byte[] GetDataFile()
        {
            FileBuilder.AddContent(FileBuilderWriter.GetHeader());

            FileBuilder.AddContent(FileBuilderWriter.GetBody());

            return FileBuilder.GetFileContent();
        }

        public IndicatorDataCsvFileBuilder(IndicatorMetadataProvider indicatorMetadataProvider, ExportAreaHelper areaHelper, IAreasReader areasReader,
            IndicatorExportParameters parameters, OnDemandQueryParametersWrapper onDemandParameters)
        {
            FileBuilder = new CsvFileBuilder();
            FileBuilderWriter = new CsvBuilderIndicatorDataWriter(areasReader, indicatorMetadataProvider, areaHelper, parameters, onDemandParameters);
        }
    }
}
