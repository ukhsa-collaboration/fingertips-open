using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export;
using PholioVisualisation.Export.File;
using PholioVisualisation.Export.FileBuilder;
using PholioVisualisation.Export.FileBuilder.Wrappers;
using PholioVisualisation.ServicesWeb.Helpers;
using System;
using System.Net.Http;

namespace PholioVisualisation.ServicesWeb.Controllers
{
    /// <summary>
    /// Class to support data controllers
    /// </summary>
    public class DataBaseController : BaseController
    {
        /// <summary>
        /// Injected GroupDataReader from factory
        /// </summary>
        protected static readonly IGroupDataReader GroupDataReader = ReaderFactory.GetGroupDataReader();

        /// <summary>
        /// Injected ProfileReader from factory
        /// </summary>
        protected static readonly IProfileReader ProfileReader = ReaderFactory.GetProfileReader();

        /// <summary>
        /// Injected ProfileReader from factory
        /// </summary>
        protected static readonly IAreasReader AreasReader = ReaderFactory.GetAreasReader();

        /// <summary>
        /// Get data response for the corresponding passed parameters
        /// </summary>
        public static HttpResponseMessage GetOnDemandIndicatorDataResponse(IAreasReader areasReader, IndicatorExportParameters exportParameters, OnDemandQueryParametersWrapper onDemandParameters)
        {
            var fileBuilder = GetOnDemandDataDataFileBuilder(areasReader, exportParameters, onDemandParameters);
            var content = fileBuilder.GetDataFile();
            return GetFileResponseMessage(exportParameters, content);
        }

        private static IndicatorDataCsvFileBuilder GetOnDemandDataDataFileBuilder(IAreasReader areasReader, IndicatorExportParameters exportParameters, OnDemandQueryParametersWrapper onDemandParameters)
        {
            var areaHelper = new ExportAreaHelper(areasReader, exportParameters);
            var fileBuilder = new IndicatorDataCsvFileBuilder(IndicatorMetadataProvider.Instance, areaHelper, areasReader,
                exportParameters, onDemandParameters);
            return fileBuilder;
        }

        private static HttpResponseMessage GetFileResponseMessage(IndicatorExportParameters exportParameters, byte[] fileContent)
        {
            if (fileContent == null)
            {
                throw new ArgumentException("Content cannot be null. It should be empty or seed.");
            }

            var filename = SingleEntityFileNamer.GetDataForUserbyIndicatorAndAreaType(exportParameters.ChildAreaTypeId);
            return FileResponseBuilder.NewMessage(fileContent, filename);
        }
    }
}