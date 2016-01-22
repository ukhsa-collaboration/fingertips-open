using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class GroupingListProvider
    {
        private IGroupDataReader groupDataReader;
        private IProfileReader profileReader;

        public GroupingListProvider(IGroupDataReader groupDataReader, IProfileReader profileReader)
        {
            this.groupDataReader = groupDataReader;
            this.profileReader = profileReader;
        }

        public IList<Grouping> GetGroupings(IList<int> profileIds, IList<int> indicatorIds, int areaTypeId)
        {
            var groupIds = profileReader.GetGroupIdsFromSpecificProfiles(profileIds);
            var groupings = groupDataReader.GetGroupingsByGroupIdsAndIndicatorIdsAndAreaType(groupIds, indicatorIds, areaTypeId);
            return new GroupingListUniquifier(groupings).GetUniqueGroupings();
        }

        public IList<Grouping> GetGroupings(IList<int> profileIds, IList<int> indicatorIds)
        {
            var groupIds = profileReader.GetGroupIdsFromSpecificProfiles(profileIds);
            var groupings = groupDataReader.GetGroupingsByGroupIdsAndIndicatorIds(groupIds, indicatorIds);
            return new GroupingListUniquifier(groupings).GetUniqueGroupings();
        }
    }
}
