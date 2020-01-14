using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class GroupRootSummaryBuilder
    {
        private IGroupDataReader _groupDataReader;
        private IndicatorMetadataProvider _metadataRepo;
        private IList<GroupRootSummary> _groupRootSummaries;

        public GroupRootSummaryBuilder(IGroupDataReader groupDataReader)
        {
            _groupDataReader = groupDataReader;
            _metadataRepo = IndicatorMetadataProvider.Instance;
            _groupRootSummaries = new List<GroupRootSummary>();
        }

        public IList<GroupRootSummary> BuildForIndicatorIds(List<int> indicatorIds, int profileId)
        {
            foreach (var indicatorId in indicatorIds)
            {
                IEnumerable<Grouping> groupings =
                    _groupDataReader.GetGroupingsByIndicatorId(indicatorId);

                // Filter groupings by profile
                if (profileId != ProfileIds.Undefined)
                {
                    var groupIds = _groupDataReader.GetGroupIdsOfProfile(profileId);
                    groupings = groupings.Where(x => groupIds.Contains(x.GroupId)).ToList();
                }

                // Distinct grouping by sex, age, indicator ID
                var distinctGroupings = GetDistinctGroupings(groupings);

                AddSummariesOfGroupings(distinctGroupings, profileId);
            }

            return _groupRootSummaries;
        }

        public IList<GroupRootSummary> BuildForProfileAndAreaType(int profileId, int areaTypeId)
        {
            IList<int> groupIds = _groupDataReader.GetGroupIdsOfProfile(profileId);
            foreach (int groupId in groupIds)
            {
                IList<Grouping> groupings =_groupDataReader
                    .GetGroupingsByGroupIdAndAreaTypeIdOrderedBySequence(groupId, areaTypeId);

                AddSummariesOfGroupings(groupings, profileId);
            }
            return _groupRootSummaries;
        }

        private static List<Grouping> GetDistinctGroupings(IEnumerable<Grouping> groupings)
        {
            var uniqueGroupings = new List<Grouping>();
            var groupedCollection = groupings.GroupBy(x => new {x.AgeId, x.SexId});
            foreach (var grouped in groupedCollection)
            {
                uniqueGroupings.Add(grouped.First());
            }
            return uniqueGroupings;
        }

        private void AddSummariesOfGroupings(IList<Grouping> groupings, int profileId)
        {
            GroupRootBuilder rootBuilder = new GroupRootBuilder(_groupDataReader);
            IList<GroupRoot> roots = rootBuilder.BuildGroupRoots(groupings);

            IndicatorMetadataCollection metadataCollection =
                _metadataRepo.GetIndicatorMetadataCollection(groupings, profileId);

            foreach (var groupRoot in roots)
            {
                IndicatorMetadata indicatorMetadata =
                    metadataCollection.GetIndicatorMetadataById(groupRoot.IndicatorId);
                if (IsDuplicate(groupRoot) == false)
                {
                    var summary = GetGroupRootSummary(groupRoot, indicatorMetadata);
                    _groupRootSummaries.Add(summary);
                }
            }
        }

        private bool IsDuplicate(GroupRoot groupRoot)
        {
            var duplicate = _groupRootSummaries.FirstOrDefault(
                x =>
                    x.Age.Id == groupRoot.AgeId &&
                    x.Sex.Id == groupRoot.SexId &&
                    x.IndicatorId == groupRoot.IndicatorId);
            return duplicate != null;
        }

        private static GroupRootSummary GetGroupRootSummary(GroupRoot groupRoot, 
            IndicatorMetadata indicatorMetadata)
        {
            return new GroupRootSummary
            {
                GroupId = groupRoot.FirstGrouping.GroupId,
                Age = groupRoot.Age,
                Sex = groupRoot.Sex,
                IndicatorId = groupRoot.IndicatorId,
                IndicatorName = indicatorMetadata.Descriptive[IndicatorMetadataTextColumnNames.Name],
                StateSex = groupRoot.StateSex,
                StateAge = groupRoot.StateAge,
                IndicatorUnit = indicatorMetadata.Unit
            };
        }
    }
}