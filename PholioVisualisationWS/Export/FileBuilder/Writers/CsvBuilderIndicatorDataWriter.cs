using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export.FileBuilder.Containers;
using PholioVisualisation.Export.FileBuilder.Wrappers;

namespace PholioVisualisation.Export.FileBuilder.Writers
{
    public class CsvBuilderIndicatorDataWriter : IFileBuilderWriter<byte[]>
    {
        private readonly IBuilderHeaderWriter<byte[]> _header;
        private readonly IBuilderBodyWriter<byte[]> _body;

        public CsvBuilderIndicatorDataWriter(IAreasReader areasReader, IndicatorMetadataProvider indicatorMetadataProvider,
            ExportAreaHelper areaHelper, IndicatorExportParameters generalParameters, OnDemandQueryParametersWrapper onDemandParameters)
        {
            var attributeForBody = new CsvBuilderAttributesForBodyContainer(generalParameters, onDemandParameters);
            _header = new CsvBuilderIndicatorDataHeaderWriter(areasReader, attributeForBody.GeneralParameters);
            _body = new CsvBuilderIndicatorDataBodyWriter(indicatorMetadataProvider, areaHelper, areasReader, attributeForBody);
        }

        public byte[] GetHeader()
        {
            return _header.GetHeader();
        }

        public byte[] GetBody()
        {
            return _body.GetBody();
        }
    }
}
