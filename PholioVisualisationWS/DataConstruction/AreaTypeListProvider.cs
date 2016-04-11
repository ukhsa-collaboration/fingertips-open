using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public interface IAreaTypeListProvider
    {
        /// <summary>
        /// Returns every area type ID used in a profile including child, parent
        /// and England area types.
        /// </summary>
        /// <param name="profileId">Profile ID</param>
        /// <returns>List of area type IDs</returns>
        List<int> GetAllAreaTypeIdsUsedInProfile(int profileId);

        IList<AreaType> GetChildAreaTypesUsedInProfiles(IEnumerable<int> profileIds);
        IList<int> GetChildAreaTypeIdsUsedInProfiles(IEnumerable<int> profileIds);
        IList<int> GetParentAreaTypeIdsUsedInProfile(int profileId);
        IList<int> GetParentAreaTypeIdsUsedInProfile(int profileId, int childAreaTypeId);
        IList<int> GetCategoryTypeIdsUsedInProfile(int profileId);
    }

    public class AreaTypeListProvider : IAreaTypeListProvider
    {
        private IGroupIdProvider groupdIdProvider;
        private IAreasReader areasReader;
        private IGroupDataReader groupDataReader;

        public AreaTypeListProvider(IGroupIdProvider groupdIdProvider, 
            IAreasReader areasReader, IGroupDataReader groupDataReader)
        {
            this.groupdIdProvider = groupdIdProvider;
            this.areasReader = areasReader;
            this.groupDataReader = groupDataReader;
        }

        public List<int> GetAllAreaTypeIdsUsedInProfile(int profileId)
        {
            var compositeChildAreaTypeIds = GetChildAreaTypeIdsUsedInProfiles(
                new List<int> { profileId });
            var childAreaTypeIds = new AreaTypeIdSplitter(compositeChildAreaTypeIds).Ids;

            var compositeParentAreaTypeIds = GetParentAreaTypeIdsUsedInProfile(profileId);
            var parentAreaTypeIds = new AreaTypeIdSplitter(compositeParentAreaTypeIds).Ids;

            var allAreaTypeIds = new List<int>();
            allAreaTypeIds.AddRange(childAreaTypeIds);
            allAreaTypeIds.AddRange(parentAreaTypeIds);
            allAreaTypeIds.Add(AreaTypeIds.Country);
            return allAreaTypeIds;
        }

        public IList<AreaType> GetChildAreaTypesUsedInProfiles(IEnumerable<int> profileIds)
        {
            return areasReader.GetAreaTypes(
                GetChildAreaTypeIdsUsedInProfiles(profileIds));
        }

        public IList<int> GetChildAreaTypeIdsUsedInProfiles(IEnumerable<int> profileIds)
        {
            var allGroupIds = GetAllGroupIdsBelongingToProfiles(profileIds);
            var areaTypeIds = groupDataReader.GetDistinctGroupingAreaTypeIds(allGroupIds);
            return areaTypeIds;
        }

        public IList<int> GetParentAreaTypeIdsUsedInProfile(int profileId, int childAreaTypeId)
        {
            var parentAreaGroups = areasReader.GetParentAreaGroups(profileId, childAreaTypeId);

            if (parentAreaGroups.Any() == false)
            {
                // Use generic parents
                parentAreaGroups = areasReader.GetParentAreaGroups(ProfileIds.Undefined, childAreaTypeId);
            }

            var parentAreaTypeIds = parentAreaGroups
                .Where(x => x.ParentAreaTypeId != null)
                .Select(x => x.ParentAreaTypeId)
                .Distinct()
                .Cast<int>()
                .ToList();

            return parentAreaTypeIds;
        }

        public IList<int> GetParentAreaTypeIdsUsedInProfile(int profileId)
        {
            var parentAreaGroups = areasReader.GetParentAreaGroups(profileId);

            var parentAreaTypeIds = parentAreaGroups
                .Where(x => x.ParentAreaTypeId != null)
                .Select(x => x.ParentAreaTypeId)
                .Distinct()
                .Cast<int>()
                .ToList();

            return parentAreaTypeIds;
        }

        public IList<int> GetCategoryTypeIdsUsedInProfile(int profileId)
        {
            var parentAreaGroups = areasReader.GetParentAreaGroups(profileId);

            var categoryTypeIds = parentAreaGroups
                .Where(x => x.CategoryTypeId != null)
                .Select(x => x.CategoryTypeId)
                .Distinct()
                .Cast<int>()
                .ToList();

            return categoryTypeIds;
        }

        private IList<int> GetAllGroupIdsBelongingToProfiles(IEnumerable<int> profileIds)
        {
            IList<int> allGroupIds = profileIds.SelectMany(profileId =>
                groupdIdProvider.GetGroupIds(profileId)).ToList();
            return allGroupIds;
        }
    }
}
