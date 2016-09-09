
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.ExceptionLogging;
using PholioVisualisation.Export;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;
using SpreadsheetGear;

namespace PholioVisualisation.Services.HttpHandlers
{
    public class GetDataDownload : IHttpHandler
    {
        public const string DownloadedFileName = "PublicHealthEngland-Data";

        private IAreasReader areasReader = ReaderFactory.GetAreasReader();

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                DataDownloadParameters parameters = new DataDownloadParameters(context.Request.Params);

                int profileId = parameters.ProfileId;
                List<int> profileIds = parameters.RestrictResultsToProfileIdList;

                var profile = ReaderFactory.GetProfileReader().GetProfile(profileId);
                int areaTypeId = parameters.AreaTypeId;
                var parentAreas = GetParentAreas(parameters, areaTypeId);
                var subnationalAreaType = AreaTypeFactory.New(areasReader, parameters.SubnationalAreaTypeId);

                ComparatorMap comparatorMap = new ComparatorMapBuilder(parentAreas).ComparatorMap;
                BaseExcelFileInfo fileInfo;
                IWorkbook workbook = null;
                ExcelFileWriter writer = new ExcelFileWriter
                {
                    UseFileCache = ApplicationConfiguration.UseFileCache
                };
                byte[] bytes = null;
                if (parameters.UseIndicatorIds)
                {
                    fileInfo = new SearchResultsFileInfo();
                    workbook = new ProfileDataBuilder(comparatorMap, profile, profileIds, 
                        parameters.IndicatorIds, parentAreas, subnationalAreaType).BuildWorkbook();
                }
                else
                {
                    fileInfo = new ProfileFileInfo(profileId, parentAreas.Select(x => x.AreaCode), areaTypeId, subnationalAreaType.Id);

                    if (ApplicationConfiguration.UseFileCache && fileInfo.DoesFileExist)
                    {
                        bytes = File.ReadAllBytes(fileInfo.FilePath);
                    }
                    else
                    {
                        workbook = new ProfileDataBuilder(comparatorMap, profile, profileIds,
                             parentAreas, subnationalAreaType).BuildWorkbook();
                    }
                }

                if (workbook != null)
                {
                    bytes = writer.Write(fileInfo, workbook);
                }

                HttpResponseBase response = new HttpContextWrapper(context).Response;
                ExportHelper.SetResponseAsExcelFile(response, DownloadedFileName + "." + fileInfo.FileExtension);
                response.BinaryWrite(bytes);
            }
            catch (Exception ex)
            {
                ExceptionLog.LogException(ex, context.Request.Url.AbsoluteUri);
            }

            context.Response.Flush();
        }

        private IList<ParentArea> GetParentAreas(DataDownloadParameters parameters, int areaTypeId)
        {
            IList<ParentArea> parentAreas = new List<ParentArea>();
            if (parameters.ParentAreaCode == "all")
            {
                // Used for MESHA profiles to provide data for 3 regions only
                var parentAreaCodes = areasReader.GetProfileParentAreaCodes(parameters.TemplateProfileId,
                    parameters.SubnationalAreaTypeId);

                foreach (var parentAreaCode in parentAreaCodes)
                {
                    parentAreas.Add(new ParentArea(parentAreaCode, areaTypeId));
                }
            }
            else
            {
                parentAreas.Add(new ParentArea(parameters.ParentAreaCode, areaTypeId));
            }
            return parentAreas;
        }
    }
}

