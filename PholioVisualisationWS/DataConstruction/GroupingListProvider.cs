using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    /// <summary>
    /// Provides list of uniquified groupings.
    /// </summary>
    public class GroupingListProvider
    {
        private IGroupDataReader _groupDataReader;
        private IProfileReader _profileReader;

        public GroupingListProvider(IGroupDataReader groupDataReader, IProfileReader profileReader)
        {
            this._groupDataReader = groupDataReader;
            this._profileReader = profileReader;
        }

        public IList<Grouping> GetGroupings(IList<int> profileIds, IList<int> indicatorIds, int areaTypeId)
        {
            var groupIds = _profileReader.GetGroupIdsFromSpecificProfiles(profileIds);
            var groupings = _groupDataReader.GetGroupingsByGroupIdsAndIndicatorIdsAndAreaType(groupIds, indicatorIds, areaTypeId);
            return new GroupingListUniquifier(groupings).GetUniqueGroupings();
        }

        public IList<Grouping> GetGroupings(IList<int> profileIds, IList<int> indicatorIds)
        {
            var groupIds = _profileReader.GetGroupIdsFromSpecificProfiles(profileIds);
            var groupings = _groupDataReader.GetGroupingsByGroupIdsAndIndicatorIds(groupIds, indicatorIds);
            return new GroupingListUniquifier(groupings).GetUniqueGroupings();
        }

        /// <summary>
        /// Returns the list of groupings.
        /// This method is used during search and indicator list
        /// </summary>
        /// <param name="indicatorIds"></param>
        /// <param name="searchMode"></param>
        /// <returns></returns>
        public IList<Grouping> GetGroupings(IList<int> indicatorIds)
        {
            // Fetch the group ids from all the profiles
            // as it is either a search or indicator list
            var groupIds = _profileReader.GetGroupIdsFromAllProfiles();

            // Retrieve the groupings based on the group ids and indicator ids
            var groupings = _groupDataReader.GetGroupingsByGroupIdsAndIndicatorIds(groupIds, indicatorIds);

            // Return unique groupings
            return new GroupingListUniquifier(groupings).GetUniqueGroupings();
        }

        public IList<Grouping> GetGroupingsByGroup(int groupId)
        {
            var groupings = _groupDataReader.GetGroupingsByGroupIds(new List<int> { groupId });
            return new GroupingListUniquifier(groupings).GetUniqueGroupings();
        }
    }
}
