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
        IList<AreaType> GetChildAreaTypesUsedInProfiles(IEnumerable<int> profileIds);
        IList<int> GetChildAreaTypeIdsUsedInProfiles(IEnumerable<int> profileIds);
        IList<int> GetParentAreaTypeIdsUsedInProfile(int profileId);
        IList<int> GetParentAreaTypeIdsUsedInProfile(int profileId, int childAreaTypeId);
        IList<int> GetParentAreaTypeIdsUsedForChildAreaType(int childAreaTypeId);
        IList<int> GetParentCategoryTypeIdsUsedInProfile(int profileId, int childAreaTypeId);
        IList<int> GetCategoryTypeIdsUsedForChildAreaType(int childAreaTypeId);
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

        public IList<AreaType> GetAllAreaTypes()
        {
            return areasReader.GetAllAreaTypes();
        }

        public IList<AreaType> GetAllAreaTypesWithData()
        {
            var areaTypeIds = groupDataReader.GetDistinctGroupingAreaTypeIdsForAllProfiles();
            return areasReader.GetAreaTypes(areaTypeIds);
        }

        public IList<AreaType> GetSupportedAreaTypes()
        {
            return areasReader.GetSupportedAreaTypes();
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
