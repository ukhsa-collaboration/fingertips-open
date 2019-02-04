using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class SingleGroupingProvider
    {
        private IGroupDataReader groupDataReader;
        private GroupIdProvider groupIdProvider;

        /// <summary>
        /// Where a particular profile is not specified then use the commmonest polarity for an indicator
        /// </summary>
        private bool useCommonestPolarity = true;

        /// <summary>
        /// Parameterless constructor for mocking
        /// </summary>
        public SingleGroupingProvider()
        {

        }

        public SingleGroupingProvider(IGroupDataReader groupDataReader, GroupIdProvider groupIdProvider)
        {
            this.groupDataReader = groupDataReader;
            this.groupIdProvider = groupIdProvider;
        }

        public virtual Grouping GetGroupingByProfileIdAndGroupIdAndAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(int profileId, int groupId,
            int areaTypeId, int indicatorId, int sexId, int ageId)
        {
            SetWhetherToUseCommonestPolarity(profileId);
            var groupIds = GetGroupIds(groupId, profileId);
            return GetGrouping(groupIds, areaTypeId, indicatorId, sexId, ageId);
        }

        public virtual Grouping GetGroupingByProfileIdAndAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(int profileId, int areaTypeId,
            GroupingDifferentiator groupingDifferentiator)
        {
            SetWhetherToUseCommonestPolarity(profileId);
            var groupIds = groupIdProvider.GetGroupIds(profileId);
            return GetGrouping(groupIds, areaTypeId, groupingDifferentiator.IndicatorId, groupingDifferentiator.SexId, groupingDifferentiator.AgeId);
        }

        public virtual Grouping GetGroupingByProfileIdAndAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(int profileId, int areaTypeId,
            int indicatorId, int sexId, int ageId)
        {
            SetWhetherToUseCommonestPolarity(profileId);
            var groupIds = groupIdProvider.GetGroupIds(profileId);
            return GetGrouping(groupIds, areaTypeId, indicatorId, sexId, ageId);
        }

        public virtual Grouping GetGroupingByGroupIdAndAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(int groupId, int areaTypeId, int indicatorId, int sexId, int ageId)
        {
            useCommonestPolarity = false;
            return GetGrouping(new List<int> { groupId }, areaTypeId, indicatorId, sexId, ageId);
        }

        public virtual Grouping GetGroupingByProfileIdAndAreaTypeIdAndIndicatorIdAndSexId(int profileId, int areaTypeId, int indicatorId, int sexId)
        {
            SetWhetherToUseCommonestPolarity(profileId);
            var groupIds = groupIdProvider.GetGroupIds(profileId);
            return GetGroupingBySexId(groupIds, areaTypeId, indicatorId, sexId);
        }

        public virtual Grouping GetGroupingByProfileIdAndAreaTypeIdAndIndicatorIdAndAgeId(
            int profileId, int areaTypeId, int indicatorId, int ageId)
        {
            SetWhetherToUseCommonestPolarity(profileId);
            var groupIds = groupIdProvider.GetGroupIds(profileId);
            return GetGroupingByAgeId(groupIds, areaTypeId, indicatorId, ageId);
        }

        public virtual Grouping GetGroupingWithLatestDataPoint(
            IList<int> groupIds, int indicatorId, int childAreaTypeId, int profileId)
        {
            SetWhetherToUseCommonestPolarity(profileId);
            var groupings = groupDataReader
                .GetGroupingsByGroupIdsAndIndicatorIds(groupIds, new List<int> { indicatorId })
                .Where(x => x.AreaTypeId == childAreaTypeId)
                .ToList();

            return ReduceGroupings(groupings);
        }

        public virtual Grouping GetGroupingWithLatestDataPointForAnyProfile(
            GroupingDifferentiator groupingDifferentiator, int childAreaTypeId)
        {
            var groupings = groupDataReader.GetGroupingsByIndicatorId(groupingDifferentiator.IndicatorId)
                .Where(x => x.AreaTypeId == childAreaTypeId &&
                        x.SexId == groupingDifferentiator.SexId &&
                        x.AgeId == groupingDifferentiator.AgeId)
                .ToList();

            return ReduceGroupings(groupings);
        }

        public virtual Grouping GetGroupingByAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(int areaTypeId,
            int indicatorId, int sexId, int ageId)
        {
            var groupings = groupDataReader.GetGroupingByAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(
                areaTypeId, indicatorId, sexId, ageId);

            return ReduceGroupings(groupings);
        }

        private Grouping GetGroupingByAgeId(IEnumerable<int> groupIds, int areaTypeId, int indicatorId, int ageId)
        {
            foreach (var groupId in groupIds)
            {
                var grouping = groupDataReader
                    .GetGroupingsByGroupIdIndicatorIdAgeId(groupId, areaTypeId, indicatorId, ageId)
                    .FirstOrDefault();

                if (grouping != null)
                {
                    return grouping;
                }
            }
            return null;
        }

        private Grouping GetGroupingBySexId(IEnumerable<int> groupIds, int areaTypeId, int indicatorId, int sexId)
        {
            foreach (var groupId in groupIds)
            {
                var grouping = groupDataReader
                    .GetGroupingsByGroupIdIndicatorIdSexId(groupId, areaTypeId, indicatorId, sexId)
                    .FirstOrDefault();

                if (grouping != null)
                {
                    return grouping;
                }
            }
            return null;
        }

        private Grouping GetGrouping(IEnumerable<int> groupIds, int areaTypeId, int indicatorId, int sexId, int ageId)
        {
            foreach (var groupId in groupIds)
            {
                var groupings = groupDataReader
                    .GetGroupings(groupId, indicatorId, areaTypeId, sexId, ageId);

                var grouping = groupings.FirstOrDefault();
                if (grouping != null)
                {
                    return ReduceGroupings(groupings);
                }
            }
            return null;
        }

        private Grouping ReduceGroupings(IList<Grouping> groupings)
        {
            if (groupings.Any() == false) return null;

            var groupingSorter = new GroupingSorter(groupings);
            var sortedGroupings = groupingSorter.SortByDataPointTimePeriodMostRecentFirst();
            var grouping = sortedGroupings.First();

            // Use most common polarity for consistency
            if (useCommonestPolarity)
            {
                var polarityId = groupDataReader.GetCommonestPolarityForIndicator(grouping.IndicatorId);
                grouping.PolarityId = polarityId;
            }

            return grouping;
        }

        private IEnumerable<int> GetGroupIds(int groupId, int profileIdToSelectGroupingsFrom)
        {
            if (groupId == GroupIds.Search)
            {
                return groupIdProvider.GetGroupIds(profileIdToSelectGroupingsFrom);
            }
            return new List<int> { groupId };
        }

        private void SetWhetherToUseCommonestPolarity(int profileId)
        {
            useCommonestPolarity = profileId == ProfileIds.Search || profileId == ProfileIds.Undefined;
        }
    }
}
