using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using PholioVisualisation.DataAccess;
using PholioVisualisation.ExceptionLogging;
using PholioVisualisation.Export;
using PholioVisualisation.RequestParameters;
using PholioVisualisation.Services.HttpHandlers;

namespace PholioVisualisation.Services
{
    /// <summary>
    /// Currently this class is only used for the Practice Profiles but expected to act as
    /// branch point for other profiles as required.
    /// </summary>
    public class DataDownloadBespoke
    {
        private HttpContextBase Context { get; set; }

        // Other possible key is "pp"
        private const string QuinaryPopulationKey = "qp";

        public DataDownloadBespoke(HttpContextBase context)
        {
            Context = context;
        }

        public void Respond()
        {
            try
            {
                DataDownloadBespokeParameters parameters = new DataDownloadBespokeParameters(Context.Request.Params);

                var fileInfo = new PracticeProfileFileInfo(parameters.ProfileKey,
                    parameters.GroupIds,
                    parameters.AreaCode);

                byte[] bytes;

                if (ApplicationConfiguration.UseFileCache && fileInfo.DoesFileExist)
                {
                    bytes = File.ReadAllBytes(fileInfo.FilePath);
                }
                else
                {
                    PracticeProfileDataBuilder builder = new PracticeProfileDataBuilder(UsePopulationData(parameters))
                    {
                        AreaCode = parameters.AreaCode,
                        GroupIds = parameters.GroupIds,
                        AreaTypeId = parameters.AreaTypeId
                    };

                    var workBook = builder.BuildWorkbook();
                    ExcelFileWriter writer = new ExcelFileWriter
                    {
                        UseFileCache = ApplicationConfiguration.UseFileCache
                    };
                    bytes = writer.Write(fileInfo, workBook);
                }

                var response = Context.Response;
                ExportHelper.SetResponseAsExcelFile(response, 
                    GetDataDownload.DownloadedFileName + "." + fileInfo.FileExtension);
                response.BinaryWrite(bytes);
            }
            catch (Exception ex)
            {
                Context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                ExceptionLog.LogException(ex, Context.Request.Url.AbsoluteUri);
            }

            Context.Response.Flush();
        }

        private static bool UsePopulationData(DataDownloadBespokeParameters parameters)
        {
            return parameters.ProfileKey.Equals(QuinaryPopulationKey,
                    StringComparison.InvariantCultureIgnoreCase);
        }
    }
}