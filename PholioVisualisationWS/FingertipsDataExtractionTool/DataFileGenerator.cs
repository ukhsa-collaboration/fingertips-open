using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.ExceptionLogging;
using PholioVisualisation.Export;
using PholioVisualisation.PholioObjects;
using NLog;
using PholioVisualisation.DataSorting;
using PholioVisualisation.Export.File;

namespace FingertipsDataExtractionTool
{
    public interface IDataFileGenerator
    {
        void Generate(int profileIdToStartFrom = 0);
    }

    public class DataFileGenerator : IDataFileGenerator
    {
        private IProfileReader _profileReader;
        private IAreasReader _areasReader;
        private IAreaTypeListProvider _areaTypeListProvider;

        private ILogger _logger;

        public DataFileGenerator(ILogger logger, IAreaTypeListProvider areaTypeListProvider,
            IAreasReader areasReader, IProfileReader profileReader)
        {
            _logger = logger;
            _areaTypeListProvider = areaTypeListProvider;
            _areasReader = areasReader;
            _profileReader = profileReader;
        }

        public void Generate(int profileIdToStartFrom = 0)
        {
            _logger.Info("About to start generating CSV files for Core Profiles");
            var allProfiles = GetProfiles(profileIdToStartFrom);

            foreach (var profile in allProfiles)
            {
                var profileId = profile.ProfileId;
                if (ShouldExcelFilesBeBuiltForProfile(profile))
                {
                    var childAreaTypeIds = GetChildAreaTypeIdsForProfile(profileId);

                    foreach (var childAreaTypeId in childAreaTypeIds)
                    {
                        var parentAreaTypeIds = _areaTypeListProvider
                            .GetParentAreaTypeIdsUsedInProfile(profileId, childAreaTypeId)
                            .ToList();

                        var categoryAreaTypeIds = _areaTypeListProvider
                            .GetParentCategoryTypeIdsUsedInProfile(profileId, childAreaTypeId)
                            .Select(CategoryAreaType.GetAreaTypeIdFromCategoryTypeId);

                        // Combine parent area type IDs with category area type IDs
                        parentAreaTypeIds.AddRange(categoryAreaTypeIds);

                        foreach (var parentAreaTypeId in parentAreaTypeIds)
                        {
                            _logger.Info("Creating Excel file with " +
                                GetProfileDetailsText(profileId,
                                parentAreaTypeId, childAreaTypeId));

                            var watch = new FileTimer(_logger);

                            CreateExcelFilesForCoreProfiles(profileId, childAreaTypeId,
                                parentAreaTypeId);

                            watch.Stop();
                        }
                    }
                }
            }
            _logger.Info("Core profiles generation completed.");
        }

        private IList<ProfileConfig> GetProfiles(int profileIdToStartFrom)
        {
            var allProfiles = _profileReader.GetAllProfiles();
            allProfiles = ProfileFilter.RemoveSystemProfiles(allProfiles);
            allProfiles = allProfiles.Where(x => x.ProfileId >= profileIdToStartFrom).ToList();
            return allProfiles;
        }

        private static bool ShouldExcelFilesBeBuiltForProfile(ProfileConfig profile)
        {
            var shouldBuild = profile.ShouldBuildExcel || profile.IsProfileLive;

            return shouldBuild && profile.IsNational;
        }

        private void CreateExcelFilesForCoreProfiles(int profileId, int areaTypeId, int subnationalAreaTypeId)
        {
            try
            {
                // Get indicatorIDs in profile
                var indicatorIds = GetIndicatorIdListProvider().GetIdsForProfile(profileId);

                var exportParameters = new IndicatorExportParameters
                {
                    ProfileId = profileId,
                    ChildAreaTypeId = areaTypeId,
                    ParentAreaTypeId = subnationalAreaTypeId,
                    ParentAreaCode = AreaCodes.England
                };

                CreateIndicatorFileContent(indicatorIds, exportParameters);
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

        private void CreateIndicatorFileContent(IList<int> indicatorIds, IndicatorExportParameters exportParameters)
        {
            var areaHelper = new ExportAreaHelper(_areasReader, exportParameters, new AreaFactory(_areasReader));
            var fileBuilder = new DataFileBuilder(IndicatorMetadataProvider.Instance,
                areaHelper, _areasReader, new DummyFileBuilder());
            fileBuilder.GetFileForSpecifiedIndicators(indicatorIds, exportParameters);
        }

        /// <summary>
        /// Remove when have DI framework
        /// </summary>
        private IIndicatorIdListProvider GetIndicatorIdListProvider()
        {
            var groupDataReader = ReaderFactory.GetGroupDataReader();
            var groupIdProvider = new GroupIdProvider(ReaderFactory.GetProfileReader());
            return new IndicatorIdListProvider(groupDataReader, groupIdProvider);
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
