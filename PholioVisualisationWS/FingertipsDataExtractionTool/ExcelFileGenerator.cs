using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.ExceptionLogging;
using PholioVisualisation.Export;
using PholioVisualisation.PholioObjects;
using SpreadsheetGear;
using NLog;

namespace FingertipsDataExtractionTool
{
    public interface IExcelFileGenerator
    {
        void Generate();
    }

    public class ExcelFileGenerator : IExcelFileGenerator
    {
        private IProfileReader _profileReader;
        private IAreasReader _areasReader;
        private IAreaTypeListProvider _areaTypeListProvider;

        private ILogger _logger;

        public ExcelFileGenerator(ILogger logger, IAreaTypeListProvider areaTypeListProvider,
            IAreasReader areasReader, IProfileReader profileReader)
        {
            _logger = logger;
            _areaTypeListProvider = areaTypeListProvider;
            _areasReader = areasReader;
            _profileReader = profileReader;
        }

        public void Generate()
        {
            _logger.Info("About to start generating excel files for Core Profiles");
            var allProfiles = _profileReader.GetAllProfiles();
            foreach (var profile in allProfiles)
            {
                if (profile.ShouldBuildExcel && profile.IsNational)
                {
                    var childAreaTypeIds = GetChildAreaTypeIdsForProfile(profile.ProfileId);

                    foreach (var childAreaTypeId in childAreaTypeIds)
                    {
                        // Ignore Practice data
                        if (childAreaTypeId != AreaTypeIds.GpPractice)
                        {
                            var parentAreaTypeIds = _areaTypeListProvider
                                .GetParentAreaTypeIdsUsedInProfile(profile.ProfileId);

                            foreach (var parentAreaTypeId in parentAreaTypeIds)
                            {
                                _logger.Info("Creating Excel file with " +
                                    GetProfileDetailsText(profile.ProfileId,
                                    parentAreaTypeId, childAreaTypeId));
                                var watch = new ExcelFileTimer(_logger);

                                CreateExcelFilesForCoreProfiles(profile.ProfileId, childAreaTypeId,
                                    parentAreaTypeId);

                                watch.Stop();
                            }
                        }
                    }
                }
            }
            _logger.Info("Core profiles generation completed.");
        }

        private void CreateExcelFilesForCoreProfiles(int profileId, int areaTypeId, int subnationalAreaTypeId)
        {
            try
            {
                var profileIds = new List<int>();

                var profile = _profileReader.GetProfile(profileId);
                var parentAreas = new List<ParentArea> { new ParentArea(AreaCodes.England, areaTypeId) };
                var subnationalAreaType = AreaTypeFactory.New(_areasReader, subnationalAreaTypeId);

                var comparatorMap = new ComparatorMapBuilder(parentAreas).ComparatorMap;

                IWorkbook workbook = new ProfileDataBuilder(comparatorMap, profile, profileIds, 0,
                        parentAreas, subnationalAreaType).BuildWorkbook();

                if (workbook != null)
                {
                    BaseExcelFileInfo fileInfo = new ProfileFileInfo(profileId,
                        parentAreas.Select(x => x.AreaCode), areaTypeId, subnationalAreaType.Id);

                    new ExcelFileWriter
                    {
                        UseFileCache = ApplicationConfiguration.UseFileCache
                    }.Write(fileInfo, workbook);
                }
            }
            catch (Exception ex)
            {
                var message = "Failed to create excel file " +
                    GetProfileDetailsText(profileId, subnationalAreaTypeId, areaTypeId);
                _logger.Error(message);
                NLogHelper.LogException(_logger, ex);
                ExceptionLog.LogException(new FingertipsException(message, ex), null);
            }
        }

        private IList<int> GetChildAreaTypeIdsForProfile(int profileId)
        {
            var childAreaTypesUsedInProfile = _areaTypeListProvider
                .GetChildAreaTypesUsedInProfiles(new List<int> { profileId });

            return childAreaTypesUsedInProfile.Select(childAreaType => childAreaType.Id).ToList();
        }

        private string GetProfileDetailsText(int profileId, int parentAreaTypeId, int childAreaTypeId)
        {
            var sb = new StringBuilder();
            sb.Append("ProfileID:")
                .Append(profileId)
                .Append(" ParentTypeID:")
                .Append(parentAreaTypeId)
                .Append(" AreaTypeID:")
                .Append(childAreaTypeId);
            return sb.ToString();
        }
    }
}
