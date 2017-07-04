using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using PholioVisualisation.DataAccess;
using PholioVisualisation.ExceptionLogging;
using PholioVisualisation.Export;
using PholioVisualisation.Export.File;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    /// <summary>
    /// Currently this class is only used for the Practice Profiles but expected to act as
    /// branch point for other profiles as required.
    /// </summary>
    public class DataDownloadBespoke
    {
        private HttpContextBase Context { get; set; }

        public const string DownloadedFileName = "PublicHealthEngland-Data";

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
                    // Get population data
                    PracticeProfileDataBuilder builder = new PracticeProfileDataBuilder()
                    {
                        AreaCode = parameters.AreaCode,
                        GroupIds = parameters.GroupIds,
                        ParentAreaTypeId = parameters.AreaTypeId
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
                    DownloadedFileName + "." + fileInfo.FileExtension);
                response.BinaryWrite(bytes);
            }
            catch (Exception ex)
            {
                Context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                ExceptionLog.LogException(ex, Context.Request.Url.AbsoluteUri);
            }

            Context.Response.Flush();
        }
    }
}