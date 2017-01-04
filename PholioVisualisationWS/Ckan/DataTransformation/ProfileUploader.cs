using Ckan.Client;
using Ckan.Model;
using Ckan.Repositories;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ckan.DataTransformation
{
    public interface IProfileUploader
    {
        void UploadProfile(int profileId);
    }

    /// <summary>
    /// Upload all the indicators in a profile to a CKAN instance.
    /// </summary>
    public class ProfileUploader : IProfileUploader
    {
        public IProfileReader profileReader = ReaderFactory.GetProfileReader();
        public IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();
        public PholioReader pholioReader = ReaderFactory.GetPholioReader();
        public IAreasReader areasReader = ReaderFactory.GetAreasReader();

        private ICkanApi ckanApi;
        private ICkanGroupRepository _ckanGroupRepository;
        private ICkanPackageRepository _ckanPackageRepository;

        private readonly List<int> indicatorIdsAlreadyUploaded = new List<int>();

        public ProfileUploader(ICkanApi ckanApi, ICkanGroupRepository _ckanGroupRepository,
            ICkanPackageRepository _ckanPackageRepository)
        {
            this.ckanApi = ckanApi;
            this._ckanGroupRepository = _ckanGroupRepository;
            this._ckanPackageRepository = _ckanPackageRepository;
        }

        public void UploadProfile(int profileId)
        {
            // Ensure a group exists
            var profile = profileReader.GetProfile(profileId);
            var ckanGroup = GetCkanGroup(profile);

            // Ckan parameters
            var groupIdProvider = new GroupIdProvider(profileReader);
            var areaTypeListProvider = new AreaTypeListProvider(groupIdProvider,
                areasReader, groupDataReader);
            var parameters = new ProfileParameters(areaTypeListProvider, profileId, ckanGroup.Name);

            var metadataRepo = IndicatorMetadataProvider.Instance;
            var areaCodesToIgnore = profileReader.GetAreaCodesToIgnore(profileId).AreaCodesIgnoredEverywhere;

            var lookUpManager = new LookUpManager(pholioReader, areasReader,
                parameters.AreaTypeIds, parameters.CategoryTypeIds);

            var groupIds = groupIdProvider.GetGroupIds(profileId);
            foreach (var groupId in groupIds)
            {
                var groupings = groupDataReader.GetGroupingsByGroupId(groupId);

                var metadataCollection = metadataRepo.GetIndicatorMetadataCollection(groupings);
                metadataRepo.RemoveSystemContentFields(metadataCollection.IndicatorMetadata);

                foreach (var grouping in groupings)
                {
                    var indicatorId = grouping.IndicatorId;
                    if (indicatorIdsAlreadyUploaded.Contains(indicatorId) == false)
                    {
                        indicatorIdsAlreadyUploaded.Add(indicatorId);

                        var timeRange = new TimeRange(grouping);

                        var metadata = metadataCollection.GetIndicatorMetadataById(indicatorId);

                        Console.WriteLine("#UPLOADING: [{0}] {1} \"{2}\"",
                            indicatorIdsAlreadyUploaded.Count, indicatorId,
                            metadata.Descriptive[IndicatorMetadataTextColumnNames.Name]
                            );

                        // Create/update package
                        var packageIdProvider = new PackageIdProvider(metadata.IndicatorId);
                        var ckanPackage = _ckanPackageRepository.RetrieveOrGetNew(packageIdProvider);
                        CkanGroup group = _ckanGroupRepository.GetExistingGroup(parameters.CkanGroupName);
                        new CkanPackagePropertySetter()
                            .SetProperties(ckanPackage, group, parameters, metadata, timeRange);
                        var savedPackage = _ckanPackageRepository.CreateOrUpdate(ckanPackage);

                        // Create metadata resource
                        var textMetadataResource = new MetadataResourceBuilder()
                            .GetUnsavedResource(savedPackage.Id, metadata, metadataRepo.IndicatorMetadataTextProperties);

                        // Create data resource
                        var allGroupingsForIndicator = groupings
                            .Where(x => x.IndicatorId == indicatorId).ToList();

                        var sexIds = allGroupingsForIndicator
                            .Select(x => x.SexId).Distinct().ToList();

                        var ageIds = groupDataReader.GetAllAgeIdsForIndicator(indicatorId);
                        var allDataList = new CoreDataListBuilder { GroupDataReader = groupDataReader }
                            .GetData(metadata.YearType, grouping, sexIds, ageIds,
                            parameters.AreaTypeIds, parameters.CategoryTypeIds, areaCodesToIgnore);

                        var dataResource = new CoreDataResourceBuilder(lookUpManager)
                            .GetUnsavedResource(savedPackage.Id, metadata, allDataList);

                        // Upload resources
                        new CkanResourceUploader { CkanApi = ckanApi }.AddResourcesToPackage(
                            savedPackage.Id, textMetadataResource, dataResource);
                    }
                }
            }
        }

        private CkanGroup GetCkanGroup(Profile profile)
        {
            CkanGroup ckanGroup = _ckanGroupRepository.CreateOrRetrieveGroup(profile);
            ckanGroup = _ckanGroupRepository.UpdateGroupWithProfileProperties(profile, ckanGroup);
            return ckanGroup;
        }
    }
}