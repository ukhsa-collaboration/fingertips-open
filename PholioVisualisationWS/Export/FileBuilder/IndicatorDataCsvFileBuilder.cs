using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export.File;
using PholioVisualisation.Export.FileBuilder.Wrappers;
using PholioVisualisation.Export.FileBuilder.Writers;

namespace PholioVisualisation.Export.FileBuilder
{
    public class IndicatorDataCsvFileBuilder : DataFileBuilderBase<byte[]>
    {
        public IndicatorDataCsvFileBuilder(IndicatorMetadataProvider indicatorMetadataProvider, ExportAreaHelper areaHelper, IAreasReader areasReader,
            IndicatorExportParameters parameters, OnDemandQueryParametersWrapper onDemandParameters) : base(new CsvFileBuilder(), 
            new CsvBuilderIndicatorDataWriter(areasReader, indicatorMetadataProvider, areaHelper, parameters, onDemandParameters))
        {
        }
    }
}
