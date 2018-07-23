using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.DataAccess.Repositories;

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
        IList<int> GetParentAreaTypeIdsUsedForChildAreaType(int childAreaTypeId);
        IList<int> GetCategoryTypeIdsForExport();
        IList<int> GetParentCategoryTypeIdsUsedInProfile(int profileId, int childAreaTypeId);
        IList<int> GetCategoryTypeIdsUsedForChildAreaType(int childAreaTypeId);
    }

    public class AreaTypeListProvider : IAreaTypeListProvider
    {
        private IGroupIdProvider groupdIdProvider;
        private IAreasReader areasReader;
        private IGroupDataReader groupDataReader;

        //TODO inject via constructor
        private IAreaTypeIdSplitter _areaTypeIdSplitter = new AreaTypeIdSplitter(new AreaTypeComponentRepository());

        public AreaTypeListProvider(IGroupIdProvider groupdIdProvider,
            IAreasReader areasReader, IGroupDataReader groupDataReader)
        {
            this.groupdIdProvider = groupdIdProvider;
            this.areasReader = areasReader;
            this.groupDataReader = groupDataReader;
        }

        public IList<AreaType> GetAllAreaTypes()
        {
            return areasReader.GetAllAreaTypes();
        }

        public IList<AreaType> GetSupportedAreaTypes()
        {
            return areasReader.GetSupportedAreaTypes();
        }

        public List<int> GetAllAreaTypeIdsUsedInProfile(int profileId)
        {
            var compositeChildAreaTypeIds = GetChildAreaTypeIdsUsedInProfiles(
                new List<int> { profileId });
            var childAreaTypeIds = _areaTypeIdSplitter.GetComponentAreaTypeIds(compositeChildAreaTypeIds);

            var compositeParentAreaTypeIds = GetParentAreaTypeIdsUsedInProfile(profileId);
            var parentAreaTypeIds = _areaTypeIdSplitter.GetComponentAreaTypeIds(compositeParentAreaTypeIds);

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

            var parentAreaTypeIds = GetDistinctParentAreaTypeIds(parentAreaGroups);
            return parentAreaTypeIds;
        }

        private static List<int> GetDistinctParentAreaTypeIds(IList<ParentAreaGroup> parentAreaGroups)
        {
            var parentAreaTypeIds = parentAreaGroups
                .Where(x => x.ParentAreaTypeId != null)
                .Select(x => x.ParentAreaTypeId)
                .Distinct()
                .Cast<int>()
                .ToList();
            return parentAreaTypeIds;
        }

        public IList<int> GetParentAreaTypeIdsUsedForChildAreaType(int childAreaTypeId)
        {
            var parentAreaGroups = areasReader.GetParentAreaGroupsForChildAreaType(childAreaTypeId);

            if (parentAreaGroups.Any())
            {
                return parentAreaGroups
                .Where(x => x.ParentAreaTypeId != null)
                .Select(x => x.ParentAreaTypeId)
                .Distinct()
                .Cast<int>()
                .ToList();
            }

            return new List<int>();
        }

        public IList<int> GetCategoryTypeIdsUsedForChildAreaType(int childAreaTypeId)
        {
            var parentAreaGroups = areasReader.GetParentAreaGroupsForChildAreaType(childAreaTypeId);

            if (parentAreaGroups.Any())
            {
                return parentAreaGroups
                .Where(x => x.CategoryTypeId != null)
                .Select(x => x.CategoryTypeId)
                .Distinct()
                .Cast<int>()
                .ToList();
            }

            return new List<int>();
        }

        public IList<int> GetParentAreaTypeIdsUsedInProfile(int profileId)
        {
            var parentAreaGroups = areasReader.GetParentAreaGroupsForProfile(profileId);

            var parentAreaTypeIds = parentAreaGroups
                .Where(x => x.ParentAreaTypeId != null)
                .Select(x => x.ParentAreaTypeId)
                .Distinct()
                .Cast<int>()
                .ToList();

            return parentAreaTypeIds;
        }

        public IList<int> GetCategoryTypeIdsForExport()
        {
            return areasReader
                .GetAllCategoryTypes()
                .Where(x => CategoryTypeIdsExcludedForExport.Ids.Contains(x.Id) == false)
                .Select(x => x.Id)
                .ToList();
        }

        public IList<int> GetParentCategoryTypeIdsUsedInProfile(int profileId, int childAreaTypeId)
        {
            var parentAreaGroups = areasReader.GetParentAreaGroupsForProfile(profileId)
                .Where(x => x.CategoryTypeId != null && x.ChildAreaTypeId == childAreaTypeId);

            return SelectDistinctCategoryTypeIds(parentAreaGroups);
        }

        private IList<int> SelectDistinctCategoryTypeIds(IEnumerable<ParentAreaGroup> parentAreaGroups)
        {
            return parentAreaGroups
                .Select(x => x.CategoryTypeId)
                .Distinct()
                .Cast<int>()
                .ToList();
        }

        private IList<int> GetAllGroupIdsBelongingToProfiles(IEnumerable<int> profileIds)
        {
            IList<int> allGroupIds = profileIds.SelectMany(profileId =>
                groupdIdProvider.GetGroupIds(profileId)).ToList();
            return allGroupIds;
        }
    }
}
